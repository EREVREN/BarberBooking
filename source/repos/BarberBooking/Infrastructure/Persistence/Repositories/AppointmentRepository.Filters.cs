using BarberBooking.Application.Appointments.Queries;
using BarberBooking.Domain.Entities;

namespace BarberBooking.Infrastructure.Persistence.Repositories;

public partial class AppointmentRepository
{
    private static IQueryable<Appointment> ApplyFilter(
        IQueryable<Appointment> query,
        AppointmentFilter filter)
    {
        if (filter.BarberId.HasValue)
            query = query.Where(a => a.BarberId == filter.BarberId);

        if (filter.Status.HasValue)
            query = query.Where(a => a.Status == filter.Status);

        if (filter.From.HasValue)
            query = query.Where(a => a.StartTime >= filter.From);

        if (filter.To.HasValue)
            query = query.Where(a => a.EndTime <= filter.To);

        return query;
    }
}


