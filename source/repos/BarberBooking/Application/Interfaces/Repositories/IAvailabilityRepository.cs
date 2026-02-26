using BarberBooking.Domain.ValueObjects;

namespace BarberBooking.Application.Interfaces.Repositories;

public interface IAvailabilityRepository
{
    Task<IReadOnlyList<TimeRange>> GetAppointments(
        Guid barberId,
        DateTime from,
        DateTime to);
}
