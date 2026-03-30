using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Entities;
using MediatR;

namespace BarberBooking.Application.Customers.Commands
{
    public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Guid>
    {

        public readonly ICustomerRepository _customerRepository;

        public CreateCustomerHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            /*var existingCustomer = await _customerRepository.GetByPhoneAsync(request.PhoneNumber);
            
            if (existingCustomer != null)
            {cancellationToken.ThrowIfCancellationRequested();
                throw new InvalidOperationException("A customer with the same phone number already exists.");
            } */
            var newCustomer = new Customer(
            
                request.Name,
                request.PhoneNumber,
                request.Email,
                request.Address
            );
            await _customerRepository.AddAsync(newCustomer);
            return newCustomer.Id;
        }
    }
}
