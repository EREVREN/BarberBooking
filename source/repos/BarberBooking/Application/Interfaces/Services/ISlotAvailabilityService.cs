using BarberBooking.Application.Models;

namespace BarberBooking.Application.Interfaces.Services
{
    public interface ISlotAvailabilityService
    {
        Task<List<AvailableSlot>> GetAvailableSlots(
        Guid barberId,
        DateTime date,
        int serviceDurationMinutes);
    }
}
