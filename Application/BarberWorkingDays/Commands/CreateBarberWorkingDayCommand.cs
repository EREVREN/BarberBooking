
using BarberBooking.Application.BarberWorkingDays.DTOs;
using MediatR;

namespace BarberBooking.Application.BarberWorkingDays.Commands
{
    public sealed record CreateBarberWorkingDayCommand (
        Guid BarberId,
        DayOfWeek DayOfWeek,
        DateTime Date,
        WorkingHoursDto WorkingHoursDto
        ) : IRequest<Guid>;

}
