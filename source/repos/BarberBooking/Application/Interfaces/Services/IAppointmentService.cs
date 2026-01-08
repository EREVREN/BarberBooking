using BarberBooking.Application.Models;

namespace BarberBooking.Application.Interfaces.Services
{
    public interface IAppointmentService
    {
        Task<BookingResult> BookAsync(
        Guid barberId,
        Guid serviceId,
        string customerPhone,
        DateTime startTime);
    }
}
