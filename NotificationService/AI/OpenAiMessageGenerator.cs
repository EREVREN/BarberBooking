namespace NotificationService.AI;

public class OpenAiMessageGenerator : IAIMessageGenerator
{
    public Task<string> GenerateConfirmationMessageAsync(
        string customerName,
        DateTime appointmentTime,
        string serviceName)
    {
        var message =
            $"Hi {customerName}, your {serviceName} appointment at {appointmentTime:f} is confirmed. We look forward to seeing you!";

        return Task.FromResult(message);
    }

    public Task<string> GenerateReminderMessageAsync(
        string customerName,
        DateTime appointmentTime,
        string serviceName)
    {
        var message =
            $"Reminder: {customerName}, your {serviceName} appointment is scheduled for {appointmentTime:f}. See you soon!";

        return Task.FromResult(message);
    }

    public async Task<DateTime> DetermineOptimalReminderTimeAsync(DateTime appointmentTime, string serviceName)
    {
        // Simulated AI analysis: Determine lead time based on service complexity and time of day
        int leadMinutes = 60; // Default 1 hour

        if (serviceName.Contains("Full", StringComparison.OrdinalIgnoreCase) ||
            serviceName.Contains("Color", StringComparison.OrdinalIgnoreCase))
        {
            leadMinutes = 120; // Complex services get 2 hours
        }

        // If it's an early morning appointment, remind slightly earlier
        if (appointmentTime.Hour <= 10)
        {
            leadMinutes += 30;
        }

        return appointmentTime.AddMinutes(-leadMinutes);
    }

    public async Task<VoiceCommandResponse> ProcessVoiceCommandAsync(string transcript, string context)
    {
        // This would typically call an LLM (like OpenAI) to parse intent
        // Returning structured steps and a natural voice response

        var transcriptLower = transcript.ToLower();

        if (transcriptLower.Contains("book") || transcriptLower.Contains("schedule"))
        {
            return new VoiceCommandResponse(
                "Sure thing! I've started the booking process for you. Which service are we looking at today?",
                new List<string> { "Detect Service Type", "Analyze Availability", "Draft Appointment" }
            );
        }

        if (transcriptLower.Contains("cancel") || transcriptLower.Contains("remove"))
        {
            return new VoiceCommandResponse(
                "I understand. I'm looking up your appointments to find the one you want to cancel.",
                new List<string> { "Identify Appointment", "Validate Cancellation Policy", "Execute Cancellation" }
            );
        }

        return new VoiceCommandResponse(
            "I'm here to help. You can ask me to book a new appointment or manage your existing ones.",
            new List<string> { "Provide Help Context" }
        );
    }
}
