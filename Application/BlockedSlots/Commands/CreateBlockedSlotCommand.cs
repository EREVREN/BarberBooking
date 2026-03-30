
using MediatR;

namespace BarberBooking.Application.BlockedSlots.Commands
{
    public sealed record CreateBlockedSlotCommand(
        Guid BarberId,
        DateTime StartTime,
        DateTime EndTime
        ) : IRequest<Guid>;

  
}
