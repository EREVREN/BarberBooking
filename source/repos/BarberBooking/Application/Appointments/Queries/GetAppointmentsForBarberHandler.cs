using MediatR;
using BarberBooking.Application.Appointments.DTOs;
using BarberBooking.Application.Interfaces.Repositories;

namespace BarberBooking.Application.Appointments.Queries;

public sealed class GetAppointmentsForBarberHandler
    : IRequestHandler<GetAppointmentsForBarberQuery, List<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;

    public GetAppointmentsForBarberHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }
    public async Task<List<AppointmentDto>> Handle(
            GetAppointmentsForBarberQuery request,
            CancellationToken cancellationToken)
    {
        var appointments = await _repository
            .GetForBarberAndDate(request.BarberId, request.Date);

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