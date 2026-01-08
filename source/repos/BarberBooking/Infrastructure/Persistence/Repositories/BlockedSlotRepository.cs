
using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Entities;
using BarberBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BarberBooking.Infrastructure.Persistence.Repositories;

public class BlockedSlotRepository : IBlockedSlotRepository
{
    private readonly BarberBookingDbContext _context;

    public BlockedSlotRepository(BarberBookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<BlockedSlot>> GetForBarberAndDate(
        Guid barberId,
        DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return await _context.Set<BlockedSlot>()
            .AsNoTracking()
            .Where(b =>
                b.BarberId == barberId &&
                b.StartTime < end &&
                b.EndTime > start)
            .ToListAsync();
    }
}