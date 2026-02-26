
using MediatR;
using BarberBooking.Application.Availability.DTOs;

namespace BarberBooking.Application.Availability.Queries;

public sealed record GetAvailabilityQuery(
    Guid BarberId,
    DateTime Date,
    int ServiceDurationMinutes
) : IRequest<AvailabilityResponse>;