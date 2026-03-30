

namespace BarberBooking.Application.Admin.Calendar.DTOs
{
    public sealed record AdminCalendarAppointmentDto(
    Guid Id,
    Guid BarberId,
    Guid CustomerId,
    Guid ServiceId,
    DateTime Start,
    DateTime End,
    string Status
);
}
