using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Enums;
using BarberBooking.Domain.ValueObjects;
using BarberBooking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace BarberBooking.Infrastructure.Repositories;

public sealed class AvailabilityRepository : IAvailabilityRepository
{
    private readonly BarberBookingDbContext _db;

    public AvailabilityRepository(BarberBookingDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TimeRange>> GetAppointments(
        Guid barberId,
        DateTime from,
        DateTime to)
    {
        return await _db.Appointments
            .Where(a =>
                a.BarberId == barberId &&
                a.StartTime < to &&
                a.EndTime > from &&
                a.Status != AppointmentStatus.Cancelled)
            .Select(a => new TimeRange(a.StartTime, a.EndTime))
            .ToListAsync();
    }
}