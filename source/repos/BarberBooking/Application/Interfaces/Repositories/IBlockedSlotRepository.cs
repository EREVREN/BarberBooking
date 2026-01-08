using BarberBooking.Domain.Entities;

namespace BarberBooking.Application.Interfaces.Repositories
{
    public interface IBlockedSlotRepository
    {
        Task<List<BlockedSlot>> GetForBarberAndDate(
        Guid barberId,
        DateTime date);
    }
}
