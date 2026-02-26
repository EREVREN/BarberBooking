using BarberBooking.Application.Customers.DTOs;
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

    public async Task<CustomerDto> GetByPhoneAsync(string phoneNumber)
    {
        return await _context.Set<CustomerDto>()
            .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
    }


    public async Task<Customer> GetByIdAsync(Guid customerId)   
        {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId);
    }
   
    public async Task<Customer> AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
        return customer;
    }
}
