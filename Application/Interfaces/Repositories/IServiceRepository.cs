using BarberBooking.Application.Services.DTOs;
using BarberBooking.Domain.Entities;

namespace BarberBooking.Application.Interfaces.Repositories
{
    public interface IServiceRepository
    {
        Task<Service?> GetByIdAsync(Guid id);
        Task<Service?> GetByNameAsync(string name);
        Task<List<ServiceDto>> GetByBarberIdAsync(Guid barberId);
        Task<List<ServiceDto>> GetAllAsync();
        Task AddAsync(Service service);
    }
}
