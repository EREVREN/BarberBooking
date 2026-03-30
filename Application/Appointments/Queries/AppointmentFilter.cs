using BarberBooking.Domain.Enums;

namespace BarberBooking.Application.Appointments.Queries;

public sealed record AppointmentFilter
{
    public Guid? BarberId { get; init; }
    public Guid? ServiceId { get; init; }
    public Guid? CustomerId { get; init; }

    public DateTime? From { get; init; }
    public DateTime? To { get; init; }

    public AppointmentStatus? Status { get; init; }
}