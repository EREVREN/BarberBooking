using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberBooking.Domain.Common.Exceptions
{
    public sealed class DuplicateServiceException : DomainException
    {
        public DuplicateServiceException(string serviceName)
            : base($"Service '{serviceName}' already exists for this barber.")
        {
        }
    }
}