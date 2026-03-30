using MediatR;
using BarberBooking.Application.Appointments.DTOs;

namespace BarberBooking.Application.Appointments.Queries;

public sealed record GetAppointmentsForBarberQuery(
    Guid BarberId,
    DateTime Date
) : IRequest<List<AppointmentDto>>;