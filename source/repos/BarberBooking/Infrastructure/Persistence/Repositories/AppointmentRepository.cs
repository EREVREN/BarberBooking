
using Application.Interfaces.Repositories;
using BarberBooking.Domain.Entities;
using BarberBooking.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace BarberBooking.Infrastructure.Persistence.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly BarberBookingDbContext _context;

    public AppointmentRepository(BarberBookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Appointment>> GetForBarberAndDate(
        Guid barberId,
        DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return await _context.Appointments
            .AsNoTracking()
            .Where(a =>
                a.BarberId == barberId &&
                a.Status != AppointmentStatus.Cancelled &&
                a.StartTime < end &&
                a.EndTime > start)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    /// <summary>
    /// Concurrency-critical overlap check
    /// </summary>
    public async Task<bool> ExistsOverlapping(
        Guid barberId,
        DateTime start,
        DateTime end)
    {
        return await _context.Appointments
            .AnyAsync(a =>
                a.BarberId == barberId &&
                a.Status != AppointmentStatus.Cancelled &&
                start < a.EndTime &&
                a.StartTime < end);
    }
}