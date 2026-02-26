

namespace BarberBooking.Application.BarberWorkingDays.DTOs
{
    public record WorkingHoursDto(
     TimeSpan StartTime,
     TimeSpan EndTime
 );
}
