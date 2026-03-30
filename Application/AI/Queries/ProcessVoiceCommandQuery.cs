using BarberBooking.Contracts.AI;
using MediatR;

namespace BarberBooking.Application.AI.Queries;

public record ProcessVoiceCommandQuery(
    string Transcript,
    string Context,
    Guid? BarberId = null,
    int? ServiceDurationMinutes = null,
    string? PreviousResponseId = null) : IRequest<VoiceCommandResponse>;
