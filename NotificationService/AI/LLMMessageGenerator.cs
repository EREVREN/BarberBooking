using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BarberBooking.Contracts.AI;

namespace NotificationService.AI;

public class LLMMessageGenerator : IAIMessageGenerator
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string _baseUrl;
    private readonly string _provider;

    private readonly string? _huggingFaceApiKey;
    private readonly string _huggingFaceBaseUrl;
    private readonly string? _huggingFaceModel;

    private readonly string? _groqApiKey;
    private readonly string _groqBaseUrl;
    private readonly string _groqModel;

    private readonly string _ollamaBaseUrl;
    private readonly string? _ollamaModel;

    private readonly string? _modalApiKey;
    private readonly string _modalBaseUrl;
    private readonly string _modalModel;

    public LLMMessageGenerator(IConfiguration config, HttpClient httpClient)
    {
        _httpClient = httpClient;

        _provider = config["LLM:Provider"] ?? "Gemini";

        _apiKey = NormalizeApiKey(config["LLM:ApiKey"]);
        _baseUrl = config["LLM:BaseUrl"] ?? "https://generativelanguage.googleapis.com/v1beta";

        _huggingFaceApiKey = NormalizeApiKey(config["HuggingFace:ApiKey"]);
        if (_huggingFaceApiKey == null &&
            string.Equals(_provider, "HuggingFace", StringComparison.OrdinalIgnoreCase))
        {
            _huggingFaceApiKey = _apiKey;
        }
        _huggingFaceBaseUrl = config["HuggingFace:BaseUrl"] ?? "https://api-inference.huggingface.co/models";
        _huggingFaceModel = config["HuggingFace:Model"];
        if (string.IsNullOrWhiteSpace(_huggingFaceModel))
        {
            _huggingFaceModel = null;
        }

        _groqApiKey = NormalizeApiKey(config["groq:ApiKey"]);
        if (_groqApiKey == null &&
            string.Equals(_provider, "groq", StringComparison.OrdinalIgnoreCase))
        {
            _groqApiKey = _apiKey;
        }
        _groqBaseUrl = (config["groq:BaseUrl"] ?? "https://api.groq.dev/v1").Trim().TrimEnd('/');
        _groqModel = (config["groq:Model"] ?? "groq/groq-1.5-mini").Trim();

        _ollamaBaseUrl = config["Ollama:BaseUrl"] ?? "http://localhost:11434";
        _ollamaModel = config["Ollama:Model"];
        if (string.IsNullOrWhiteSpace(_ollamaModel))
        {
            _ollamaModel = null;
        }
    
        _modalApiKey = NormalizeApiKey(config["Modal:ApiKey"]);
        if (_modalApiKey == null &&
            (string.Equals(_provider, "Modal", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(_provider, "TensorRTLLM", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(_provider, "TensorRT-LLM", StringComparison.OrdinalIgnoreCase)))
        {
            _modalApiKey = _apiKey;
        }
        _modalBaseUrl = (config["Modal:BaseUrl"] ?? "").Trim().TrimEnd('/');
        _modalModel = (config["Modal:Model"] ?? "llama3-8b").Trim();}

    private static string? NormalizeApiKey(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var trimmed = value.Trim();

        if (trimmed.Equals("HuggingFace_API_KEY", StringComparison.OrdinalIgnoreCase)) return null;
        if (trimmed.Equals("GEMINI_API_KEY", StringComparison.OrdinalIgnoreCase)) return null;
        if (trimmed.StartsWith("process.env.", StringComparison.OrdinalIgnoreCase)) return null;

        return trimmed;
    }

    public async Task<string> GenerateConfirmationMessageAsync(
        string customerName,
        DateTime appointmentTime,
        string serviceName)
    {
        var prompt = $"Generate a friendly barber shop appointment confirmation message for {customerName}. " +
                     $"Service: {serviceName}, Time: {appointmentTime:f}. Keep it concise and professional.";

        return await CallApiAsync(prompt) ?? $"Hi {customerName}, your {serviceName} is confirmed for {appointmentTime:f}.";
    }

    public async Task<string> GenerateReminderMessageAsync(
        string customerName,
        DateTime appointmentTime,
        string serviceName)
    {
        var prompt = $"Generate a friendly appointment reminder for {customerName}. " +
                     $"Service: {serviceName}, Time: {appointmentTime:f}. Mention we are excited to see them.";

        return await CallApiAsync(prompt) ?? $"Reminder: {customerName}, your {serviceName} is at {appointmentTime:f}.";
    }

    public async Task<DateTime> DetermineOptimalReminderTimeAsync(DateTime appointmentTime, string serviceName)
    {
        var prompt = $"Given an appointment for '{serviceName}' at {appointmentTime:f}, " +
                     $"return ONLY a JSON object with a field 'minutesBefore' representing how many minutes before the appointment " +
                     $"the reminder should be sent for optimal customer preparation.";

        var response = await CallApiAsync(prompt, expectsJson: true);

        try
        {
            if (response != null)
            {
                var match = System.Text.RegularExpressions.Regex.Match(response, @"\d+");
                if (match.Success && int.TryParse(match.Value, out int minutes))
                {
                    return appointmentTime.AddMinutes(-minutes);
                }
            }
        }
        catch { /* Fallback */ }

        return appointmentTime.AddHours(-1);
    }

    public async Task<VoiceCommandResponse> ProcessVoiceCommandAsync(string transcript, string context)
    {
        var prompt = $"The user said: '{transcript}'. Current context: {context}. " +
                     "Analyze the intent. Return ONLY a JSON with: " +
                     "'spokenResponse' (what to say back), 'steps' (array of strings describing technical steps to take), " +
                     "and optional 'scheduledTime' (ISO string if they want to book).";

        var response = await CallApiAsync(prompt, expectsJson: true);

        if (!string.IsNullOrWhiteSpace(response))
        {
            try
            {
                var jsonString = ExtractJsonObject(response);
                if (jsonString != null)
                {
                    using var doc = JsonDocument.Parse(jsonString);
                    var root = doc.RootElement;

                    var spoken = root.TryGetProperty("spokenResponse", out var spokenValue) &&
                                 spokenValue.ValueKind == JsonValueKind.String
                        ? spokenValue.GetString()
                        : null;

                    var steps = new List<string>();
                    if (root.TryGetProperty("steps", out var stepsValue) &&
                        stepsValue.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var step in stepsValue.EnumerateArray())
                        {
                            if (step.ValueKind == JsonValueKind.String)
                            {
                                steps.Add(step.GetString() ?? string.Empty);
                            }
                            else if (step.ValueKind != JsonValueKind.Null &&
                                     step.ValueKind != JsonValueKind.Undefined)
                            {
                                steps.Add(step.GetRawText());
                            }
                        }
                        steps.RemoveAll(s => string.IsNullOrWhiteSpace(s));
                    }

                    DateTime? scheduledTime = null;
                    if (root.TryGetProperty("scheduledTime", out var scheduledValue))
                    {
                        if (scheduledValue.ValueKind == JsonValueKind.String &&
                            DateTime.TryParse(scheduledValue.GetString(), out var parsed))
                        {
                            scheduledTime = parsed;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(spoken) || steps.Count > 0 || scheduledTime != null)
                    {
                        return new VoiceCommandResponse(
                            spoken ?? "Okay.",
                            steps,
                            scheduledTime);
                    }
                }
            }
            catch
            {
                // fall through to raw response fallback
            }
        }

        if (!string.IsNullOrWhiteSpace(response))
        {
            return new VoiceCommandResponse(response.Trim(), new List<string>());
        }

        return new VoiceCommandResponse("I'm sorry, I couldn't process that command.", new List<string> { "Error Parsing" });
    }

    private async Task<string?> CallApiAsync(string prompt, bool expectsJson = false)
    {
        if (string.Equals(_provider, "Ollama", StringComparison.OrdinalIgnoreCase))
        {
            return await CallOllamaAsync(prompt, expectsJson);
        }

        if (string.Equals(_provider, "HuggingFace", StringComparison.OrdinalIgnoreCase))
        {
            return await CallHuggingFaceAsync(prompt);
        }

        if (string.Equals(_provider, "groq", StringComparison.OrdinalIgnoreCase))
        {
            return await CallGroqAsync(prompt, expectsJson);
        }
        if (string.Equals(_provider, "Modal", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(_provider, "TensorRTLLM", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(_provider, "TensorRT-LLM", StringComparison.OrdinalIgnoreCase))
        {
            return await CallModalAsync(prompt, expectsJson);
        }

        return await CallGeminiAsync(prompt);
    }

    private async Task<string?> CallModalAsync(string prompt, bool expectsJson)
    {
        if (_modalApiKey == null || string.IsNullOrWhiteSpace(_modalBaseUrl))
        {
            return null;
        }

        var url = $"{_modalBaseUrl}/chat/completions";

        var requestBody = new Dictionary<string, object?>
        {
            ["model"] = _modalModel,
            ["messages"] = new[]
            {
                new Dictionary<string, object?>
                {
                    ["role"] = "user",
                    ["content"] = prompt
                }
            },
            ["temperature"] = 0.2,
            ["max_tokens"] = 256
        };

        if (expectsJson)
        {
            requestBody["response_format"] = new Dictionary<string, object?> { ["type"] = "json_object" };
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _modalApiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("choices", out var choices) &&
                choices.ValueKind == JsonValueKind.Array &&
                choices.GetArrayLength() > 0)
            {
                var choice0 = choices[0];
                if (choice0.ValueKind == JsonValueKind.Object &&
                    choice0.TryGetProperty("message", out var message) &&
                    message.ValueKind == JsonValueKind.Object &&
                    message.TryGetProperty("content", out var content) &&
                    content.ValueKind == JsonValueKind.String)
                {
                    return content.GetString();
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
    private async Task<string?> CallGroqAsync(string prompt, bool expectsJson)
    {
        if (_groqApiKey == null)
        {
            return null;
        }

        var url = $"{_groqBaseUrl}/chat/completions";

        var requestBody = new Dictionary<string, object?>
        {
            ["model"] = _groqModel,
            ["messages"] = new[]
            {
                new Dictionary<string, object?>
                {
                    ["role"] = "user",
                    ["content"] = prompt
                }
            },
            ["temperature"] = 0.2,
            ["max_tokens"] = 256
        };

        if (expectsJson)
        {
            requestBody["response_format"] = new Dictionary<string, object?> { ["type"] = "json_object" };
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _groqApiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("choices", out var choices) &&
                choices.ValueKind == JsonValueKind.Array &&
                choices.GetArrayLength() > 0)
            {
                var choice0 = choices[0];
                if (choice0.ValueKind == JsonValueKind.Object &&
                    choice0.TryGetProperty("message", out var message) &&
                    message.ValueKind == JsonValueKind.Object &&
                    message.TryGetProperty("content", out var content) &&
                    content.ValueKind == JsonValueKind.String)
                {
                    return content.GetString();
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> CallGeminiAsync(string prompt)
    {
        if (_apiKey == null)
        {
            // LLM key not configured; callers will fall back to non-AI responses.
            return null;
        }

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"{_baseUrl.TrimEnd('/')}/models/gemini-1.5-flash:generateContent?key={_apiKey}";

        try
        {
            var response = await _httpClient.PostAsync(url, content);

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
        catch { return null; }
    }

    private async Task<string?> CallHuggingFaceAsync(string prompt)
    {
        if (_huggingFaceApiKey == null || _huggingFaceModel == null)
        {
            return null;
        }

        var url = $"{_huggingFaceBaseUrl.TrimEnd('/')}/{_huggingFaceModel}";

        var requestBody = new
        {
            inputs = prompt,
            parameters = new
            {
                max_new_tokens = 256,
                return_full_text = false
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _huggingFaceApiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 0)
            {
                var first = doc.RootElement[0];
                if (first.ValueKind == JsonValueKind.Object &&
                    first.TryGetProperty("generated_text", out var generatedText) &&
                    generatedText.ValueKind == JsonValueKind.String)
                {
                    return generatedText.GetString();
                }
            }

            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                if (doc.RootElement.TryGetProperty("generated_text", out var generatedText) &&
                    generatedText.ValueKind == JsonValueKind.String)
                {
                    return generatedText.GetString();
                }

                if (doc.RootElement.TryGetProperty("error", out var errorValue) &&
                    errorValue.ValueKind == JsonValueKind.String)
                {
                    return null;
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> CallOllamaAsync(string prompt, bool expectsJson)
    {
        if (_ollamaModel == null)
        {
            return null;
        }

        var url = $"{_ollamaBaseUrl.TrimEnd('/')}/api/generate";
        var requestBody = new
        {
            model = _ollamaModel,
            prompt,
            stream = false,
            format = expectsJson ? "json" : null,
            options = new
            {
                num_predict = 256
            }
        };

        try
        {
            var response = await _httpClient.PostAsync(
                url,
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode) return null;

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("response", out var responseValue) &&
                responseValue.ValueKind == JsonValueKind.String)
            {
                return responseValue.GetString();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private class LLMVoiceResponse
    {
        public string SpokenResponse { get; set; } = "";
        public List<string> Steps { get; set; } = new();
        public DateTime? ScheduledTime { get; set; }
    }

    private static string? ExtractJsonObject(string input)
    {
        var cleaned = input.Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "", StringComparison.OrdinalIgnoreCase)
            .Trim();

        var start = cleaned.IndexOf('{');
        var end = cleaned.LastIndexOf('}');
        if (start < 0 || end <= start) return null;

        return cleaned.Substring(start, end - start + 1);
    }
}
