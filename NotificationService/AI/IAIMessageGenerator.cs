namespace NotificationService.AI;

public record VoiceCommandResponse(
    string SpokenResponse,
    List<string> Steps,
    DateTime? ScheduledTime = null);

public interface IAIMessageGenerator
{
    Task<string> GenerateConfirmationMessageAsync(
        string customerName,
        DateTime appointmentTime,
        string serviceName);

    Task<string> GenerateReminderMessageAsync(
        string customerName,
        DateTime appointmentTime,
        string serviceName);

    /// <summary>
    /// Determines the optimal reminder time using AI analysis based on service type and time.
    /// </summary>
    Task<DateTime> DetermineOptimalReminderTimeAsync(DateTime appointmentTime, string serviceName);

    /// <summary>
    /// Processes a voice command transcript and returns a voice-ready response with workflow steps.
    /// </summary>
    Task<VoiceCommandResponse> ProcessVoiceCommandAsync(string transcript, string context);
}
