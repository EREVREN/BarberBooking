using BarberBooking.Application.BarberWorkingDays.DTOs;
using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Interfaces.Repositories;

using MediatR;

namespace BarberBooking.Application.BarberWorkingDays.Queries
{

    internal sealed class GetForBarberAndDateHandler
        : IRequestHandler<GetForBarberAndDateQuery, BarberWorkingDayDto?>
    {

        public readonly IBarberWorkingDayRepository _barberWorkingDayRepository;
        public GetForBarberAndDateHandler(IBarberWorkingDayRepository barberWorkingDayRepository)
        {
            _barberWorkingDayRepository = barberWorkingDayRepository;
        }
        public async Task<BarberWorkingDayDto?> Handle(GetForBarberAndDateQuery request, CancellationToken cancellationToken)
        {
            var barberWorkingDay = await _barberWorkingDayRepository.GetForBarberAndDate(
                request.BarberId, 
                request.Date
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
