
using BarberBooking.Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment appointment);

        Task<List<Appointment>> GetForBarberAndDate(
            Guid barberId,
            DateTime date);

        Task<bool> ExistsOverlapping(
            Guid barberId,
            DateTime start,
            DateTime end);
    }
}
