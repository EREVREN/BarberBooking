
using BarberBooking.Domain.Common.Exceptions;

namespace BarberBooking.Domain.Entities
{
    public class Barber
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;

        public Barber(string name)
        {
            Id = Guid.NewGuid();
            Name = name;
        }

        private readonly List<Service> _services = new();
        public IReadOnlyCollection<Service> Services => _services.AsReadOnly();
        public void AddService(Service service)
        {
            if (_services.Any(s => s.Name == service.Name))
                throw new DuplicateServiceException(service.Name);

            if (service.DurationMinutes <= 0)
                throw new InvalidServiceDurationException();

            _services.Add(service);
        }

    }
}
