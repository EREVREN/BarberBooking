
using BarberBooking.Application.Appointments.Queries;
using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Entities;
using BarberBooking.Domain.Enums;
using BarberBooking.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BarberBooking.Infrastructure.Persistence.Repositories;

public partial class AppointmentRepository : IAppointmentRepository
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
    public async Task<IReadOnlyList<TimeRange>> GetAppointmentsForBarberOnDate(
        Guid barberId,
        DateTime date)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        return await _context.Appointments
            .AsNoTracking()
            .Where(a =>
                a.BarberId == barberId &&
                a.Status != AppointmentStatus.Cancelled &&
                a.StartTime < dayEnd &&
                a.EndTime > dayStart)
            .OrderBy(a => a.StartTime)
            .Select(a => new TimeRange(
                a.StartTime,
                a.EndTime))
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

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        return await _context.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }
    public async Task<List<Appointment>> GetAllAsync()
    {
        return await _context.Appointments
            .AsNoTracking()
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }
    public async Task<int> CountAsync()
    {
        return await _context.Appointments.CountAsync();
    }

    public async Task<List<Appointment>> GetPagedAsync(int skip, int take)
    {
        return await _context.Appointments
            .AsNoTracking()
            .OrderByDescending(a => a.StartTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
    public async Task<int> CountAsync(AppointmentFilter filter)
    {
        return await ApplyFilter(_context.Appointments, filter)
            .CountAsync();
    }

    public async Task<List<Appointment>> GetPagedAsync(
        AppointmentFilter filter,
        int skip,
        int take)
    {
        return await ApplyFilter(_context.Appointments, filter)
            .AsNoTracking()
            .OrderByDescending(a => a.StartTime)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }
}