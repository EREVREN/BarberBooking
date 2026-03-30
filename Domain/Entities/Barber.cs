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

        private readonly List<BarberService> _barberServices = new();
        public IReadOnlyCollection<BarberService> BarberServices => _barberServices.AsReadOnly();

        public void AddService(Guid serviceId)
        {
            if (_barberServices.Any(s => s.ServiceId == serviceId))
                return;

            _barberServices.Add(new BarberService(this.Id, serviceId));
        }
    }
}
