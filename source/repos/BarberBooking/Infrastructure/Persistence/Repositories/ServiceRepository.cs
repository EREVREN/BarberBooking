using BarberBooking.Application.Interfaces.Repositories;
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
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Service?> GetByNameAsync(string name)
    {
        return await _context.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == name);
    }
}
