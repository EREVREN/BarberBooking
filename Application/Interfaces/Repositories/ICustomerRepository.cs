

using BarberBooking.Application.Customers.DTOs;
using BarberBooking.Domain.Entities;

namespace BarberBooking.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(Guid customerId);
        Task<CustomerDto> GetByPhoneAsync(string phoneNumber);
        Task<Customer> AddAsync (Customer customer);
        


    }
}
