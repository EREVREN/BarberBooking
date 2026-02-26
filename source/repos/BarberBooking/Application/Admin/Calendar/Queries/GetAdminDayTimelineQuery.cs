using BarberBooking.Application.Admin.Calendar.DTOs;
using MediatR;

namespace BarberBooking.Application.Admin.Calendar.Queries;

public sealed record GetAdminDayTimelineQuery(
    DateTime Date,
    Guid? BarberId
) : IRequest<AdminDayTimelineDto>;