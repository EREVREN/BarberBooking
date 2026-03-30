
namespace BarberBooking.Application.Availability.DTOs;

public sealed record AvailabilityResponse(
    Guid BarberId,
    DateTime Date,
    IReadOnlyList<AvailabilitySlot> Slots
);