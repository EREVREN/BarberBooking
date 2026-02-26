
namespace BarberBooking.Application.Appointments.DTOs;

public sealed record AdminAppointmentDto(
    Guid Id,
    Guid BarberId,
    Guid ServiceId,
    Guid CustomerId,
    DateTime StartTime,
    DateTime EndTime,
    string Status
);
