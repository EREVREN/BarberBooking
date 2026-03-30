using MediatR;

namespace BarberBooking.Application.Customers.Commands
{
    public sealed record CreateCustomerCommand(
        string Name,
        string PhoneNumber,
        string? Email,
        string? Address
    ) : IRequest<Guid>;
    
}
