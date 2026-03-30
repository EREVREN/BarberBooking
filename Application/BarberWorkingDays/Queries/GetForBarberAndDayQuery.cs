using BarberBooking.Application.BarberWorkingDays.DTOs;
using MediatR;

namespace BarberBooking.Application.BarberWorkingDays.Queries
{
    public sealed record GetForBarberAndDayQuery (
        Guid BarberId,
        DayOfWeek DayOfWeek
        ) : IRequest<BarberWorkingDayDto?>;
      
}
