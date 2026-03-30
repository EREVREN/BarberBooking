using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NotificationService.AI;

public class GeminiMessageGenerator : IAIMessageGenerator
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string GeminiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

    public GeminiMessageGenerator(IConfiguration config, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = config["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey is missing");
    }

    public async Task<string> GenerateConfirmationMessageAsync(
        string customerName,
        DateTime appointmentTime,
        string serviceName)
    {
        var prompt = $"Generate a friendly barber shop appointment confirmation message for {customerName}. " +
                     $"Service: {serviceName}, Time: {appointmentTime:f}. Keep it concise and professional.";

        return await CallGeminiAsync(prompt) ?? $"Hi {customerName}, your {serviceName} is confirmed for {appointmentTime:f}.";
    }

    public async Task<string> GenerateReminderMessageAsync(
        string customerName,
        DateTime appointmentTime,
        string serviceName)
    {
        var prompt = $"Generate a friendly appointment reminder for {customerName}. " +
                     $"Service: {serviceName}, Time: {appointmentTime:f}. Mention we are excited to see them.";

        return await CallGeminiAsync(prompt) ?? $"Reminder: {customerName}, your {serviceName} is at {appointmentTime:f}.";
    }

    public async Task<DateTime> DetermineOptimalReminderTimeAsync(DateTime appointmentTime, string serviceName)
    {
        var prompt = $"Given an appointment for '{serviceName}' at {appointmentTime:f}, " +
                     $"return ONLY a JSON object with a field 'minutesBefore' representing how many minutes before the appointment " +
                     $"the reminder should be sent for optimal customer preparation. Consider service complexity.";

        var response = await CallGeminiAsync(prompt);

        try
        {
            // Simple parsing logic for AI response
            if (response != null && response.Contains("minutesBefore"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(response, @"\""minutesBefore\""\s*:\s*(\d+)");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int minutes))
                {
                    return appointmentTime.AddMinutes(-minutes);
                }
            }
        }
        catch { /* Fallback */ }

        return appointmentTime.AddHours(-1); // Fallback
    }

    public async Task<VoiceCommandResponse> ProcessVoiceCommandAsync(string transcript, string context)
    {
        var prompt = $"The user said: '{transcript}'. Current context: {context}. " +
                     "Analyze the intent. Return ONLY a JSON with: " +
                     "'spokenResponse' (what to say back), 'steps' (array of technical steps to take), " +
                     "and optional 'scheduledTime' (ISO string if they want to book).";

        var response = await CallGeminiAsync(prompt);

        try
        {
            if (response != null)
            {
                // Clean markdown if AI returns it
                var jsonString = response.Replace("```json", "").Replace("```", "").Trim();
                var result = JsonSerializer.Deserialize<GeminiVoiceResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result != null)
                {
                    return new VoiceCommandResponse(
                        result.SpokenResponse,
                        result.Steps ?? new List<string>(),
                        result.ScheduledTime);
                }
            }
        }
        catch { /* Fallback */ }

        return new VoiceCommandResponse("I'm sorry, I couldn't process that request. How else can I help?", new List<string> { "Error Parsing Command" });
    }

    private async Task<string?> CallGeminiAsync(string prompt)
    {
        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{GeminiUrl}?key={_apiKey}", content);

        if (!response.IsSuccessStatusCode) return null;

        var responseJson = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseJson);

        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();
    }

    private class GeminiVoiceResponse
    {
        public string SpokenResponse { get; set; } = "";
        public List<string> Steps { get; set; } = new();
        public DateTime? ScheduledTime { get; set; }
    }
}
