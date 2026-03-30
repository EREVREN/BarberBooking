using BarberBooking.Application.Admin.Calendar.DTOs;
using MediatR;

namespace BarberBooking.Application.Admin.Calendar.Queries
{
    public sealed record GetAdminWeekTimelineQuery(
    DateTime WeekStart,
    Guid? BarberId
) : IRequest<AdminWeekTimelineDto>;

}
