using MediatR;


namespace BarberBooking.Contracts.Events
{
    public record BookingReminderEvent
(
    Guid AppointmentId,
    string CustomerEmail,
    string ServiceName,
    DateTime StartTime,
    DateTime ReminderTime
): IRequest<Guid>;
}
