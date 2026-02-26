

namespace BarberBooking.Application.Admin.Calendar.DTOs
{
    
    public sealed record AdminDayTimelineDto(
    DateTime Date,
    IReadOnlyList<AdminCalendarAppointmentDto> Appointments);
}

