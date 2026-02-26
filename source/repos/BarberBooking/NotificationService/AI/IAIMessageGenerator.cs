namespace NotificationService.AI;

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
}
