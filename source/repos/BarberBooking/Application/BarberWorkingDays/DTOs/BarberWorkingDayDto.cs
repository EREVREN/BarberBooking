using BarberBooking.Domain.ValueObjects;

namespace BarberBooking.Application.BarberWorkingDays.DTOs
{
    public record BarberWorkingDayDto (
        Guid Id,
        Guid BarberId,
        DayOfWeek? DayOfWeek,
        DateTime? Date,
        WorkingHours WorkingHours);

}
