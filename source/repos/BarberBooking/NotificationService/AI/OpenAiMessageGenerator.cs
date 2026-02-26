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
}
