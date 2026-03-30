using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Application.Services.DTOs;
using BarberBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberBooking.Infrastructure.Persistence.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly BarberBookingDbContext _context;

    public ServiceRepository(BarberBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Service?> GetByIdAsync(Guid id)
    {
        return await _context.Services
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Service?> GetByNameAsync(string name)
    {
        return await _context.Services
            .FirstOrDefaultAsync(s => s.Name == name);
    }
   
    public async Task<List<ServiceDto>> GetByBarberIdAsync(Guid barberId)
    {
        // Fixed: Use the new BarberService join table instead of EF.Property shadow property
        return await _context.BarberServices
            .Where(bs => bs.BarberId == barberId)
            .Select(bs => bs.Service)
            .Select(s => new ServiceDto(
                s.Id,
                s.Name,
                s.DurationMinutes,
                s.Price
            ))
            .ToListAsync();
    }

    public async Task<List<ServiceDto>> GetAllAsync()
    {
        return await _context.Services
            .Select(s => new ServiceDto(
                s.Id,
                s.Name,
                s.DurationMinutes,
                s.Price
            ))
            .ToListAsync();
    }

    public async Task AddAsync(Service service)
    {
        await _context.Services.AddAsync(service);
        await _context.SaveChangesAsync();
    }
}
