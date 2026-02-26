using BarberBooking.Application.Appointments.DTOs;

namespace BarberBooking.Application.Appointments.Queries;

public sealed record AdminAppointmentsResult(
    IReadOnlyList<AdminAppointmentDto> Items,
    int TotalCount
);