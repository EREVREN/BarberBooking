using MediatR;
using BarberBooking.Application.Appointments.DTOs;

namespace BarberBooking.Application.Appointments.Queries;

public sealed record GetAppointmentByIdQuery(Guid Id)
    : IRequest<AppointmentDto?>;