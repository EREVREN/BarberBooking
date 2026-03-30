using MediatR;
using BarberBooking.Application.Appointments.DTOs;
using BarberBooking.Application.Interfaces.Repositories;


namespace BarberBooking.Application.Appointments.Queries;

public sealed class GetAppointmentByIdHandler
    : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto?>
{
    private readonly IAppointmentRepository _repository;

    public GetAppointmentByIdHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<AppointmentDto?> Handle(
        GetAppointmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var appointment = await _repository.GetByIdAsync(request.Id);

        if (appointment is null)
            return null;

        return new AppointmentDto(
            appointment.Id,
            appointment.BarberId,
            appointment.ServiceId,
            appointment.CustomerId,
            appointment.StartTime,
            appointment.EndTime,
            appointment.Status
        );
    }
}
