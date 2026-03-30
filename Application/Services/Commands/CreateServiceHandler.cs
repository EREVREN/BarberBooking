using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Entities;
using MediatR;


namespace BarberBooking.Application.Services.Commands
{
    public class CreateServiceHandler
      : IRequestHandler<CreateServiceCommand, Guid>
    {
        private readonly IBarberRepository _barberRepository;
        private readonly IServiceRepository _serviceRepository;


        public CreateServiceHandler(IBarberRepository barberRepository, 
            IServiceRepository serviceRepository)
        {
            _barberRepository = barberRepository;
            _serviceRepository = serviceRepository;
        }
        public async Task<Guid> Handle(
               CreateServiceCommand request,
               CancellationToken cancellationToken)
        {
            var barber = await _barberRepository.GetByIdAsync(request.BarberId)
                ?? throw new Exception("Barber not found");

            var service = new Service(
                request.Name,
                request.DurationMinutes,
                request.Price
            );

           // barber.AddService(service);

           //await _barberRepository.UpdateAsync(barber);

            await _serviceRepository.AddAsync(service);
            return service.Id;

        }
    }
}
