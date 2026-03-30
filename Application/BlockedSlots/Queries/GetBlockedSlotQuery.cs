

using BarberBooking.Application.BlockedSlots.DTOs;
using MediatR;

namespace BarberBooking.Application.BlockedSlots.Queries
{
    public sealed record GetBlockedSlotQuery (
        Guid BarberId, 
        DateTime Date
        ) : IRequest<List<BlockedSlotDto?>>;

}
