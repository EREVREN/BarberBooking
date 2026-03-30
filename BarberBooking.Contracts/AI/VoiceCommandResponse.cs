namespace BarberBooking.Contracts.AI;

public record VoiceCommandResponse(
    string SpokenResponse,
    List<string> Steps,
    DateTime? ScheduledTime = null,
    string? ResponseId = null);
