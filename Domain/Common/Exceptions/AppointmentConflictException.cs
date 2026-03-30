using BarberBooking.Domain.Common.Exceptions;

namespace BarberBooking.Domain.Exceptions;

public sealed class AppointmentConflictException : DomainException
{
    public AppointmentConflictException()
        : base("Selected time slot is no longer available")
    {
    }
}
