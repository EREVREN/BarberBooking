using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Application.Services.DTOs;
using MediatR;

namespace BarberBooking.Application.Services.Queries;

public sealed class GetAllServicesHandler : IRequestHandler<GetAllServicesQuery, List<ServiceDto>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetAllServicesHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<List<ServiceDto>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
    {
        return await _serviceRepository.GetAllAsync();
    }
}
