using MediatR;
using BarberBooking.Application.Barbers.DTOs;

namespace BarberBooking.Application.Barbers.Queries;

public record GetBarbersQuery : IRequest<List<BarberDto>>;