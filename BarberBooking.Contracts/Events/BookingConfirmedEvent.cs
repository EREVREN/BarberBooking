using MediatR;

namespace BarberBooking.Contracts.Events;

public record BookingConfirmedEvent(
    Guid AppointmentId,
    Guid BarberId,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string ServiceName,
    DateTime StartTime,
    DateTime EndTime,
    DateTime CreatedAt
);
