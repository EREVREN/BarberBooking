

namespace BarberBooking.Application.Services.DTOs
{
    public record ServiceDto(
    Guid Id,
    string Name,
    int DurationMinutes,
    decimal Price
);
}
