

namespace BarberBooking.Application.Availability.DTOs;

public sealed record AvailabilitySlot(
    DateTime Start,
    DateTime End,
    bool isAvailable
);