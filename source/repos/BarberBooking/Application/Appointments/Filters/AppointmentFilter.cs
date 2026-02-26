using BarberBooking.Domain.Enums;

namespace BarberBooking.Application.Appointments.Filters;
    public sealed class AppointmentFilter
    {
        public Guid? BarberId { get; init; }
        public AppointmentStatus? Status { get; init; }
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }
    }

