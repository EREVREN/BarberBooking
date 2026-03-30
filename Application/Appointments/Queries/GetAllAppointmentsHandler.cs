using MediatR;
using BarberBooking.Application.Appointments.DTOs;
using BarberBooking.Application.Interfaces.Repositories;

namespace BarberBooking.Application.Appointments.Queries;

public sealed class GetAllAppointmentsHandler
    : IRequestHandler<GetAllAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;

    public GetAllAppointmentsHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AppointmentDto>> Handle(
        GetAllAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var appointments = await _repository.GetAllAsync();

        return appointments
            .Select(a => new AppointmentDto(
                a.Id,
                a.BarberId,
                a.ServiceId,
                a.CustomerId,
                a.StartTime,
                a.EndTime,
                a.Status
            ))
            .ToList();
    }
}