using MediatR;
using BarberBooking.Application.Appointments.DTOs;

namespace BarberBooking.Application.Appointments.Queries;

public sealed record GetAllAppointmentsQuery
    : IRequest<List<AppointmentDto>>;