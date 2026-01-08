using BarberBooking.Domain.Entities;

namespace BarberBooking.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByPhoneAsync(string phoneNumber);
        Task AddAsync(Customer customer);
    }
}
