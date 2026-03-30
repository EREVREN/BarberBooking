
using MediatR;
namespace BarberBooking.Application.Services.Commands
{
   

    public record CreateServiceCommand(
        Guid BarberId,
        string Name,
        int DurationMinutes,
        decimal Price
    ) : IRequest<Guid>;
}
