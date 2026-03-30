using MediatR;

namespace BarberBooking.Application.Appointments.Commands;
public record ConfirmAppointmentCommand(
    Guid BarberId,
    Guid CustomerId,
    Guid ServiceId,
    DateTime StartTime,
    DateTime EndTime
) : IRequest<Guid>;
