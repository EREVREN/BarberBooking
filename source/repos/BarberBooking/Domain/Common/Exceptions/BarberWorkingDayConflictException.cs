using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberBooking.Domain.Common.Exceptions
{
    public class BarberWorkingDayConflictException : DomainException
    {
        public BarberWorkingDayConflictException()
            : base("Barber already has a working day scheduled for the selected date.")
        {
        }
    }
}
