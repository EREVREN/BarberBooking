using BarberBooking.Domain.Entities;

namespace BarberBooking.Application.Interfaces.Repositories
{
    public interface IWorkingDayRepository
    {
        Task<BarberWorkingDay?> GetForBarberAndDay(
        Guid barberId,
        DayOfWeek dayOfWeek);
    }
}
