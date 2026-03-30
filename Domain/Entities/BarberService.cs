namespace BarberBooking.Domain.Entities;

public class BarberService
{
    public Guid BarberId { get; private set; }
    public Guid ServiceId { get; private set; }

    // Navigation properties
    public Barber Barber { get; private set; } = null!;
    public Service Service { get; private set; } = null!;

    protected BarberService() { }

    public BarberService(Guid barberId, Guid serviceId)
    {
        BarberId = barberId;
        ServiceId = serviceId;
    }
}
