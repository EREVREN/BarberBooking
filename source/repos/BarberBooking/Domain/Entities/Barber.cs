namespace BarberBooking.Domain.Entities
{
    public class Barber
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;

        private readonly List<Service> _services = new();
        public IReadOnlyCollection<Service> Services => _services;
    }
}
