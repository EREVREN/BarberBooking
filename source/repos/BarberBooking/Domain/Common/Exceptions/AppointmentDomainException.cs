
using BarberBooking.Domain.Common.Exceptions;

namespace BarberBooking.Domain.Exceptions;

public sealed class AppointmentDomainException : DomainException
{
    public AppointmentDomainException(string message)
        : base(message)
    {
    }
}