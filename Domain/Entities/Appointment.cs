
using BarberBooking.Domain.Enums;
using BarberBooking.Domain.Exceptions;

namespace BarberBooking.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public Guid BarberId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid ServiceId { get; private set; }

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public AppointmentStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    // ✅ EF Core ONLY
    protected Appointment() { }

    // ✅ Domain constructor (EF will NOT use this)
    private Appointment(
        Guid barberId,
        Guid customerId,
        Guid serviceId,
        DateTime startTime,
        DateTime endTime,
        AppointmentStatus status)
    {

        if (startTime >= endTime)
            throw new AppointmentDomainException("StartTime must be before EndTime");

        Id = Guid.NewGuid();
        BarberId = barberId;
        CustomerId = customerId;
        ServiceId = serviceId;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }

    
       

    public void Confirm()
    {
        if (Status != AppointmentStatus.Pending)
            throw new AppointmentDomainException("Only pending appointments can be confirmed");

        Status = AppointmentStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == AppointmentStatus.Cancelled)
            return;

        Status = AppointmentStatus.Cancelled;
    }

    // ✅ Domain factory (THIS is what you call)
    public static Appointment Create(
        Guid barberId,
        Guid customerId,
        Guid serviceId,
        DateTime startTime,
        DateTime endTime,
        AppointmentStatus status)
    {
        if (startTime >= endTime)
            throw new InvalidOperationException("Invalid appointment time range");

        return new Appointment(
            barberId,
            customerId,
            serviceId,
            startTime,
            endTime,
            status);
    }
}
   
   