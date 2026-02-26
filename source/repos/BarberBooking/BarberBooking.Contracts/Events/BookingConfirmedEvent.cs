using MediatR;

namespace BarberBooking.Contracts.Events
{
    public record BookingConfirmedEvent
(
    Guid AppointmentId,
    Guid BarberId,
    Guid CustomerId,
    string CustomerEmail,
    string CustomerPhone,
    string ServiceName,
    DateTime StartTime,
    DateTime EndTime,
    DateTime CreatedAt
) : IRequest<Guid>;
}
