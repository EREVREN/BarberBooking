using BarberBooking.Domain.Entities;
using BarberBooking.Domain.Interfaces.Repositories;
using BarberBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BarberBooking.Infrastructure.Repositories;

public sealed class BarberWorkingDayRepository : IBarberWorkingDayRepository
{
    private readonly BarberBookingDbContext _context;

    public BarberWorkingDayRepository(BarberBookingDbContext context)
    {
        _context = context;
    }

    // 1️⃣ Weekly working hours (Mon–Sun)
    public async Task<BarberWorkingDay?> GetForBarberAndDay(
        Guid barberId,
        DayOfWeek dayOfWeek)
    {
        return await _context.BarberWorkingDays
            .AsNoTracking()
            .Where(x =>
                x.BarberId == barberId &&
                x.DayOfWeek == dayOfWeek &&
                x.Date == null)
            .FirstOrDefaultAsync();
    }

    // 2️⃣ Date override → fallback to weekly
    public async Task<BarberWorkingDay?> GetForBarberAndDate(
        Guid barberId,
        DateTime date)
    {
        var day = date.Date;

        // 1️⃣ Check override first
        var overrideDay = await _context.BarberWorkingDays
            .AsNoTracking()
            .Where(x =>
                x.BarberId == barberId &&
                x.Date == day)
            .FirstOrDefaultAsync();

        if (overrideDay != null)
            return overrideDay;

        // 2️⃣ Fallback to weekly
        return await GetForBarberAndDay(barberId, day.DayOfWeek);
    }
    public async Task<bool> ExistsOverlapping(
        Guid barberId,
        DateTime date)
    {
        // Check if override already exists
        return await _context.BarberWorkingDays
            .AnyAsync(x =>
                x.BarberId == barberId &&
                x.Date == date.Date);


    }
    public async Task<BarberWorkingDay> AddAsync(BarberWorkingDay workingDay, CancellationToken cancellationToken)
    {
        await _context.BarberWorkingDays.AddAsync(workingDay, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return workingDay;
    }
}
