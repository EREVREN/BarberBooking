using BarberBooking.Application.Barbers.DTOs;
using MediatR;

namespace BarberBooking.Application.Barbers.Queries;

public record GetBarbersByServiceQuery(Guid ServiceId) : IRequest<List<BarberDto>>;
