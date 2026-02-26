using BarberBooking.Application.Admin.Calendar.DTOs;
using BarberBooking.Application.Appointments.Queries;
using BarberBooking.Application.Interfaces.Repositories;
using MediatR;

namespace BarberBooking.Application.Admin.Calendar.Queries;

public sealed class GetAdminDayTimelineHandler
    : IRequestHandler<GetAdminDayTimelineQuery, AdminDayTimelineDto>
{
    private readonly IAppointmentRepository _repository;

    public GetAdminDayTimelineHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdminDayTimelineDto> Handle(
        GetAdminDayTimelineQuery request,
        CancellationToken ct)
    {
       
        var start = request.Date.Date;
        var end = start.AddDays(1);

        var filter = new AppointmentFilter
        {
            BarberId = request.BarberId,
            From = start,
            To = end
        };

        var appointments = await _repository.GetPagedAsync(
            filter,
            skip: 0,
            take: 500 // safe admin cap
        );

        var dtos = appointments.Select(a =>
            new AdminCalendarAppointmentDto(
                a.Id,
                a.BarberId,
                a.CustomerId,
                a.ServiceId,
                a.StartTime,
                a.EndTime,
                a.Status.ToString()
            )).ToList();

        return new AdminDayTimelineDto(start, dtos);
    }
}
