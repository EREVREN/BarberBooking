namespace BarberBooking.Application.Models
{
    public class BookingResult
    {
        public Guid AppointmentId { get; init; }
        public DateTime StartTime { get; init; }
        public DateTime EndTime { get; init; }
        public string Status { get; init; } = default!;
    }
}
