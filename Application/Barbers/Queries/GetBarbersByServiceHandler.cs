using BarberBooking.Application.Barbers.DTOs;
using BarberBooking.Application.Interfaces.Repositories;
using MediatR;

namespace BarberBooking.Application.Barbers.Queries;

public sealed class GetBarbersByServiceHandler : IRequestHandler<GetBarbersByServiceQuery, List<BarberDto>>
{
    private readonly IBarberRepository _barberRepository;

    public GetBarbersByServiceHandler(IBarberRepository barberRepository)
    {
        _barberRepository = barberRepository;
    }

    public async Task<List<BarberDto>> Handle(GetBarbersByServiceQuery request, CancellationToken cancellationToken)
    {
        return await _barberRepository.GetByServiceIdAsync(request.ServiceId);
    }
}
