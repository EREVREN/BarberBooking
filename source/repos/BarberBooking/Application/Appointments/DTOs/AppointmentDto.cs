using BarberBooking.Domain.Entities;
using BarberBooking.Domain.Enums;

namespace BarberBooking.Application.Appointments.DTOs;

public sealed record AppointmentDto(
    Guid Id,
    Guid BarberId,
    Guid ServiceId,
    Guid CustomerId,
    DateTime StartTime,
    DateTime EndTime,
    AppointmentStatus Status
)
{
    public static AppointmentDto From(Appointment appointment)
        => new(
            appointment.Id,
            appointment.BarberId,
            appointment.ServiceId,
            appointment.CustomerId,
            appointment.StartTime,
            appointment.EndTime,
            appointment.Status
        );
}


