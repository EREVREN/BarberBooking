using MediatR;


namespace BarberBooking.Application.Barbers.Commands
{
    public record CreateBarberCommand(string Name) : IRequest<Guid>;
}
