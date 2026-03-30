using BarberBooking.Application.Barbers.DTOs;
using BarberBooking.Domain.Entities;

namespace BarberBooking.Application.Interfaces.Repositories;

public interface IBarberRepository
{
    Task AddAsync(Barber barber);
    Task UpdateAsync(Barber barber);
    Task<Barber?> GetByIdAsync(Guid id);
    Task<Barber?> GetByNameAsync(string name);
    Task<List<Barber>> GetAllAsync();
    Task<List<BarberDto>> GetAllAsDtoAsync();
    Task<List<BarberDto>> GetByServiceIdAsync(Guid serviceId);
}
