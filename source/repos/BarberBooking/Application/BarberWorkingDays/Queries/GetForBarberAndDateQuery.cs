using BarberBooking.Application.BarberWorkingDays.DTOs;
using MediatR;

namespace BarberBooking.Application.BarberWorkingDays.Queries
{
    public sealed record GetForBarberAndDateQuery(
        Guid BarberId,
        DateTime Date
        ) : IRequest<BarberWorkingDayDto?>;

}
