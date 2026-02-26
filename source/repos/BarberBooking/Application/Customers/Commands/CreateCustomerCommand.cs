using MediatR;

namespace BarberBooking.Application.Customers.Commands
{
    public sealed record CreateCustomerCommand(
        string FirstName,
        string LastName,
        string PhoneNumber,
        string? Email,
        string? Address
    ) : IRequest<Guid>;
    
}
