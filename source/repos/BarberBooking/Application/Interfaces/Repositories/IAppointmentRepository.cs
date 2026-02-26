
using BarberBooking.Application.Appointments.Queries;
using BarberBooking.Domain.Entities;
using BarberBooking.Domain.ValueObjects;

namespace BarberBooking.Application.Interfaces.Repositories
{
    public interface IAppointmentRepository
    {
        Task AddAsync(Appointment appointment);
        Task<Appointment?> GetByIdAsync(Guid id);
        Task<List<Appointment>> GetAllAsync();
        Task<List<Appointment>> GetForBarberAndDate( Guid barberId, DateTime date );   
        Task<IReadOnlyList<TimeRange>> GetAppointmentsForBarberOnDate(Guid barberId,DateTime date);
        Task<bool> ExistsOverlapping(Guid barberId,DateTime start,DateTime end);
        Task<List<Appointment>> GetPagedAsync(AppointmentFilter filter, int skip, int take);
        Task<int> CountAsync();
      
        Task<int> CountAsync(AppointmentFilter filter);
        
    }
}
