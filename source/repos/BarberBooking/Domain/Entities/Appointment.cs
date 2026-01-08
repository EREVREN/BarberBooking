using BarberBooking.Domain.Enums;

namespace BarberBooking.Domain.Entities
{
    public class Appointment : Common.BaseEntity
    {
        public Guid BarberId { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid ServiceId { get; private set; }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public AppointmentStatus Status { get; private set; }

        protected Appointment() { }

        public Appointment(
            Guid barberId,
            Guid customerId,
            Guid serviceId,
            DateTime start,
            DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("Invalid time range");

            BarberId = barberId;
            CustomerId = customerId;
            ServiceId = serviceId;
            StartTime = start;
            EndTime = end;
            Status = AppointmentStatus.Pending;
        }

        public void Confirm() => Status = AppointmentStatus.Confirmed;
        public void Cancel() => Status = AppointmentStatus.Cancelled;
    }
}
