using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Application.Services.DTOs;
using MediatR;

namespace BarberBooking.Application.Services.Queries
{


    public sealed class GetBarberServicesHandler
        : IRequestHandler<GetBarberServicesQuery, List<ServiceDto>>
    {
        private readonly IServiceRepository _serviceRepository;

        public GetBarberServicesHandler(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public async Task<List<ServiceDto>> Handle(
            GetBarberServicesQuery request,
            CancellationToken cancellationToken)
        {
            var services = await _serviceRepository
                .GetByBarberIdAsync(request.BarberId);

            return services
                .Select(s => new ServiceDto(
                    s.Id,
                    s.Name,
                    s.DurationMinutes,
                    s.Price
                ))
                .ToList();
        }
    }
}