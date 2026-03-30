using BarberBooking.Application.Barbers.DTOs;
using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberBooking.Infrastructure.Persistence.Repositories;

public class BarberRepository : IBarberRepository
{
    private readonly BarberBookingDbContext _context;

    public BarberRepository(BarberBookingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Barber barber)
    {
        _context.Barbers.Add(barber);
        await _context.SaveChangesAsync();
    }

    public async Task<List<BarberDto>> GetAllAsDtoAsync()
    {
        return await _context.Barbers
            .AsNoTracking()
            .Select(b => new BarberDto(
                b.Id,
                b.Name
            ))
            .ToListAsync();
    }

    public async Task<List<Barber>> GetAllAsync()
    {
        return await _context.Barbers.ToListAsync();
    }

    public async Task<Barber?> GetByIdAsync(Guid id)
    {
        return await _context.Barbers
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Barber?> GetByNameAsync(string name)
    {
        return await _context.Barbers
            .FirstOrDefaultAsync(b => b.Name == name);
    }

    public async Task UpdateAsync(Barber barber)
    {
        _context.Barbers.Update(barber);
        await _context.SaveChangesAsync();
    }

    public async Task<List<BarberDto>> GetByServiceIdAsync(Guid serviceId)
    {
        return await _context.BarberServices
            .Where(bs => bs.ServiceId == serviceId)
            .Select(bs => bs.Barber)
            .Select(b => new BarberDto(
                b.Id,
                b.Name
            ))
            .ToListAsync();
    }
}
