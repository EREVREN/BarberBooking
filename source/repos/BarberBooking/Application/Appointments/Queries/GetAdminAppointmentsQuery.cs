
using MediatR;

namespace BarberBooking.Application.Appointments.Queries;

public sealed record GetAdminAppointmentsQuery(
    AppointmentFilter Filter,
    int Page,
    int PageSize
) : IRequest<AdminAppointmentsResult>;
