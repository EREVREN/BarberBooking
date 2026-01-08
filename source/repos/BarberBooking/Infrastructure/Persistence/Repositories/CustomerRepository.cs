using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarberBooking.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly BarberBookingDbContext _context;

    public CustomerRepository(BarberBookingDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByPhoneAsync(string phoneNumber)
    {
        return await _context.Set<Customer>()
            .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Set<Customer>().AddAsync(customer);
        await _context.SaveChangesAsync();
    }
}
