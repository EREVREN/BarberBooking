using BarberBooking.Application.Appointments.DTOs;
using BarberBooking.Application.Appointments.Queries;
using BarberBooking.Application.Common.Pagination;
using BarberBooking.Application.Interfaces.Repositories;
using MediatR;

public sealed class GetAppointmentsHandler
    : IRequestHandler<GetAppointmentsQuery, PagedResult<AppointmentDto>>
{
    private readonly IAppointmentRepository _repository;

    public GetAppointmentsHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<AppointmentDto>> Handle(
        GetAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        var totalCount = await _repository.CountAsync(request.Filter);

        var appointments = await _repository.GetPagedAsync(
            request.Filter,
            request.Skip,
            request.PageSize);

        return new PagedResult<AppointmentDto>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Items = appointments.Select(AppointmentDto.From).ToList()

           
        };
    }
}