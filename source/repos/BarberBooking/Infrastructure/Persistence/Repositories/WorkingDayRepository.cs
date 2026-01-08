using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberBooking.Infrastructure.Persistence.Repositories;

public class WorkingDayRepository : IWorkingDayRepository
{
    private readonly BarberBookingDbContext _context;

    public WorkingDayRepository(BarberBookingDbContext context)
    {
        _context = context;
    }

    public async Task<BarberWorkingDay?> GetForBarberAndDay(
        Guid barberId,
        DayOfWeek dayOfWeek)
    {
        return await _context.Set<BarberWorkingDay>()
            .AsNoTracking()
            .FirstOrDefaultAsync(w =>
                w.BarberId == barberId &&
                w.DayOfWeek == dayOfWeek);
    }
}