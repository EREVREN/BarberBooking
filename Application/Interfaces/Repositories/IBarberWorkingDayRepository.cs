
using BarberBooking.Domain.Entities;

namespace BarberBooking.Domain.Interfaces.Repositories
{
    public interface IBarberWorkingDayRepository
    {

        Task<BarberWorkingDay?> GetForBarberAndDay(
            Guid barberId,
            DayOfWeek dayOfWeek);
        Task<BarberWorkingDay?> GetForBarberAndDate(
            Guid barberId,
            DateTime date);
        Task<bool> ExistsOverlapping(
            Guid barberId,
            DateTime date);
        Task<BarberWorkingDay> AddAsync(BarberWorkingDay workingDay, CancellationToken cancellation);

    }
}