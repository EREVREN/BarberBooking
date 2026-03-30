namespace BarberBooking.Contracts.AI;

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

    Task<DateTime> DetermineOptimalReminderTimeAsync(
        DateTime appointmentTime,
        string serviceName);

    Task<VoiceCommandResponse> ProcessVoiceCommandAsync(
        string transcript,
        string context,
        string? previousResponseId = null);
}
