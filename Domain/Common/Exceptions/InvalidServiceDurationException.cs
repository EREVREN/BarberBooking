using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberBooking.Domain.Common.Exceptions
{
    public sealed class InvalidServiceDurationException : DomainException
    {
        public InvalidServiceDurationException()
            : base("Service duration must be greater than zero.")
        {
        }
    }
}
