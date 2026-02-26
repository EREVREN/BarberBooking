using BarberBooking.Application.Common.Pagination;
using BarberBooking.Application.Appointments.DTOs;

namespace BarberBooking.Application.Appointments.Queries;

public sealed record GetAppointmentsPagedQuery(
    AppointmentFilter Filter,
    int Page = 1,
    int PageSize = 20
) : PagedQuery<PagedResult<AppointmentDto>>(Page, PageSize);