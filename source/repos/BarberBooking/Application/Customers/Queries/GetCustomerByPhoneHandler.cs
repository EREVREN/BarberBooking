using MediatR;
using BarberBooking.Application.Customers.DTOs;
using BarberBooking.Application.Interfaces.Repositories;

namespace BarberBooking.Application.Customers.Queries
{
    public class GetCustomerByPhoneHandler : IRequestHandler<GetCustomerByPhoneQuery, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;
        public GetCustomerByPhoneHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<CustomerDto> Handle(GetCustomerByPhoneQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByPhoneAsync(request.PhoneNumber);
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found with the provided phone number.");
            }
            return new CustomerDto
            (
                customer.Id,
               customer.FirstName,
                customer.LastName,
                customer.PhoneNumber,
                customer.Email,
                customer.Address
            );
        }

    }
}
