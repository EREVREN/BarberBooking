using BarberBooking.Application.Barbers.DTOs;
using BarberBooking.Application.Services.DTOs;
using MediatR;

namespace BarberBooking.Application.Services.Queries
{

    public record GetBarberServicesQuery(Guid BarberId)
        : IRequest<List<ServiceDto>>;
}
