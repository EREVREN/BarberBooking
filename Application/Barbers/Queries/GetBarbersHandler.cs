using MediatR;
using BarberBooking.Application.Barbers.DTOs;
using BarberBooking.Application.Interfaces.Repositories;

namespace BarberBooking.Application.Barbers.Queries;

public class GetBarbersHandler
    : IRequestHandler<GetBarbersQuery, List<BarberDto>>
{
    private readonly IBarberRepository _barberRepository;

    public GetBarbersHandler(IBarberRepository barberRepository)
    {
        _barberRepository = barberRepository;
    }

    public async Task<List<BarberDto>> Handle(
    GetBarbersQuery request,
    CancellationToken cancellationToken)
    {
        return await _barberRepository.GetAllAsDtoAsync();
    }
}