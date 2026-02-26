using BarberBooking.Application.Appointments.DTOs;
using BarberBooking.Application.Common.Pagination;

namespace BarberBooking.Application.Appointments.Queries;

public sealed record GetAppointmentsQuery(
    
    AppointmentFilter Filter,
    int Page = 1,
    int PageSize = 20
) : PagedQuery<PagedResult<AppointmentDto>>(Page, PageSize);