
using BarberBooking.Domain.Common.Exceptions;
using BarberBooking.Domain.Entities;
using BarberBooking.Domain.Interfaces.Repositories;
using BarberBooking.Domain.ValueObjects;
using MediatR;

namespace BarberBooking.Application.BarberWorkingDays.Commands;

internal sealed class CreateBarberWorkingDayHandler
        : IRequestHandler<CreateBarberWorkingDayCommand, Guid>
{
    public readonly IBarberWorkingDayRepository _barberWorkingDayRepository;

    public CreateBarberWorkingDayHandler(IBarberWorkingDayRepository barberWorkingDayRepository)
    {
        _barberWorkingDayRepository = barberWorkingDayRepository;
    }

    public async Task<Guid> Handle(CreateBarberWorkingDayCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine("🔥 WORKINGDAY HANDLER HIT");
        var hasConflict = await _barberWorkingDayRepository.ExistsOverlapping(
        request.BarberId,
        request.Date);

        if (hasConflict)
            throw new BarberWorkingDayConflictException();

        var barberWorkingDay = new BarberWorkingDay(
                request.BarberId,
                request.DayOfWeek,
                request.Date,

            request.WorkingHoursDto == null ? null : new WorkingHours(
                request.WorkingHoursDto.StartTime,
                request.WorkingHoursDto.EndTime
                
                ));

             

        var created = await _barberWorkingDayRepository.AddAsync(barberWorkingDay, cancellationToken);
        return created.Id;
    }
}
