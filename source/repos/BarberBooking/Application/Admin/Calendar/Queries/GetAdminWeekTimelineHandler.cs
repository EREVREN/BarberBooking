using BarberBooking.Application.Admin.Calendar.DTOs;
using BarberBooking.Application.Appointments.Queries;
using BarberBooking.Application.Interfaces.Repositories;
using MediatR;

namespace BarberBooking.Application.Admin.Calendar.Queries;

public sealed class GetAdminWeekTimelineHandler
    : IRequestHandler<GetAdminWeekTimelineQuery, AdminWeekTimelineDto>
{
    private readonly IAppointmentRepository _repository;

    public GetAdminWeekTimelineHandler(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<AdminWeekTimelineDto> Handle(
        GetAdminWeekTimelineQuery request,
        CancellationToken ct)
    {
        
        var start = request.WeekStart.Date;
        var end = start.AddDays(7);

        var filter = new AppointmentFilter
        {
            BarberId = request.BarberId,
            From = start,
            To = end
        };

        var appointments = await _repository.GetPagedAsync(
            filter,
            skip: 0,
            take: 2000
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

        return new AdminWeekTimelineDto(start, end, dtos);
    }
}
