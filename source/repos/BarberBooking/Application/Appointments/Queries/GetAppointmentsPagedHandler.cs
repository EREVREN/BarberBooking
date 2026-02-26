using MediatR;
using BarberBooking.Application.Common.Pagination;
using BarberBooking.Application.Appointments.DTOs;
using BarberBooking.Application.Interfaces.Repositories;

namespace BarberBooking.Application.Appointments.Queries;

public sealed class GetAppointmentsPagedHandler
    : IRequestHandler<GetAppointmentsPagedQuery, PagedResult<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;

    public GetAppointmentsPagedHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<AppointmentDto>> Handle(
        GetAppointmentsPagedQuery request,
        CancellationToken cancellationToken)
    {

        var totalCount = await _repository.CountAsync(request.Filter);



        var appointments = await _repository.GetPagedAsync(
            filter : request.Filter,
            skip : request.Skip,
            take : request.PageSize);

        return new PagedResult<AppointmentDto>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Items = appointments.Select(a => new AppointmentDto(
                a.Id,
                a.BarberId,
                a.ServiceId,
                a.CustomerId,
                a.StartTime,
                a.EndTime,
                a.Status
            )).ToList()
        };
    }
}