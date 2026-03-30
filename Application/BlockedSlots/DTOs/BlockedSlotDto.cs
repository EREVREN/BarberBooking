

namespace BarberBooking.Application.BlockedSlots.DTOs
{
    public sealed record BlockedSlotDto (
        Guid BarberId,
        DateTime StartTime,
        DateTime EndTime
        );
    
}
