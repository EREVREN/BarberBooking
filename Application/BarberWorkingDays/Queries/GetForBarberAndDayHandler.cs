using BarberBooking.Application.BarberWorkingDays.DTOs;
using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Interfaces.Repositories;

using MediatR;

namespace BarberBooking.Application.BarberWorkingDays.Queries
{

    public sealed class GetForBarberAndDayHandler
        : IRequestHandler<GetForBarberAndDayQuery, BarberWorkingDayDto?>
    {

        public readonly IBarberWorkingDayRepository _barberWorkingDayRepository;
        public GetForBarberAndDayHandler(IBarberWorkingDayRepository barberWorkingDayRepository)
        {
            _barberWorkingDayRepository = barberWorkingDayRepository;
        }
        public async Task<BarberWorkingDayDto?> Handle(GetForBarberAndDayQuery request, CancellationToken cancellationToken)
        {
            var barberWorkingDay = await _barberWorkingDayRepository.GetForBarberAndDay(
                request.BarberId,
                request.DayOfWeek
               );

            if (barberWorkingDay is null)
                return null;

            return new BarberWorkingDayDto(
                barberWorkingDay.Id,
                barberWorkingDay.BarberId,
                barberWorkingDay.DayOfWeek,
                barberWorkingDay.Date,
                barberWorkingDay.WorkingHours);

        }
    }
}
