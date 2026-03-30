using BarberBooking.Domain.Common;

namespace BarberBooking.Domain.Entities
{
    public class Service : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public int DurationMinutes { get; private set; }
        public decimal Price { get; private set; }

        private readonly List<BarberService> _barberServices = new();
        public IReadOnlyCollection<BarberService> BarberServices => _barberServices.AsReadOnly();

        protected Service() { }

        public Service(string name, int durationMinutes, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Service name is required");

            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be positive");

            if (price < 0)
                throw new ArgumentException("Price cannot be negative");
           
            Name = name;
            DurationMinutes = durationMinutes;
            Price = price;
        }
    }
}
