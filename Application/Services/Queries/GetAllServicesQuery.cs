using BarberBooking.Application.Services.DTOs;
using MediatR;

namespace BarberBooking.Application.Services.Queries;

public record GetAllServicesQuery() : IRequest<List<ServiceDto>>;
