using BarberBooking.Application.Appointments.DTOs;
using BarberBooking.Application.Interfaces.Repositories;
using MediatR;

namespace BarberBooking.Application.Appointments.Queries;

public sealed class GetAdminAppointmentsHandler
    : IRequestHandler<GetAdminAppointmentsQuery, AdminAppointmentsResult>
{
    private readonly IAppointmentRepository _repository;

    public GetAdminAppointmentsHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdminAppointmentsResult> Handle(
        GetAdminAppointmentsQuery request,
        CancellationToken ct)
    {
        var skip = (request.Page - 1) * request.PageSize;

        var total = await _repository.CountAsync(request.Filter);

        var appointments = await _repository.GetPagedAsync(
            request.Filter,
            skip,
            request.PageSize);

        var items = appointments.Select(a => new AdminAppointmentDto(
            a.Id,
            a.BarberId,
            a.ServiceId,
            a.CustomerId,
            a.StartTime,
            a.EndTime,
            a.Status.ToString()
        )).ToList();

        return new AdminAppointmentsResult(items, total);
    }
}