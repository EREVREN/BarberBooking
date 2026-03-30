using BarberBooking.Application.Availability.DTOs;
using BarberBooking.Application.Interfaces.Repositories;
using BarberBooking.Domain.Interfaces.Repositories;
using BarberBooking.Domain.ValueObjects;
using MediatR;

namespace BarberBooking.Application.Availability.Queries;

public sealed class GetAvailabilityHandler
    : IRequestHandler<GetAvailabilityQuery, AvailabilityResponse>
{
    private readonly IBarberWorkingDayRepository _workingDayRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IBlockedSlotRepository _blockedSlotRepository;

    public GetAvailabilityHandler(
        IBarberWorkingDayRepository workingDayRepository,
        IAppointmentRepository appointmentRepository,
        IBlockedSlotRepository blockedSlotRepository)
    {
        _workingDayRepository = workingDayRepository;
        _appointmentRepository = appointmentRepository;
        _blockedSlotRepository = blockedSlotRepository;
    }

    public async Task<AvailabilityResponse> Handle(
        GetAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        // 1️⃣ Get working hours (override → weekly fallback)
        Console.WriteLine("🔥 AVAILABILITY HANDLER HIT");
        var workingDay = await _workingDayRepository
            .GetForBarberAndDate(request.BarberId, request.Date);

        if (workingDay == null)
        {
            return new AvailabilityResponse(
                request.BarberId,
                request.Date,
                Array.Empty<AvailabilitySlot>());
        }

        // 2️⃣ Build day range using TimeSpan → DateTime (FIXES YOUR ERRORS)
        var dayStart = request.Date.Date + workingDay.WorkingHours.Start;
        var dayEnd = request.Date.Date + workingDay.WorkingHours.End;

        var workingRange = new TimeRange(dayStart, dayEnd);
       
        var blocked = await _blockedSlotRepository
            .GetForBarberAndDate(
                request.BarberId,
                request.Date);

        // 3️⃣ Get existing appointments
        var appointments = await _appointmentRepository
            .GetAppointmentsForBarberOnDate( 
                request.BarberId,
                request.Date);

        var busyRanges = appointments
            .Select(a => new TimeRange(a.Start, a.End))
            .Concat(blocked.Select(b => new TimeRange(b.StartTime, b.EndTime)))
            .ToList();

        // 4️⃣ Generate slots
        var slots = GenerateSlots(
            workingRange,
            appointments,
            busyRanges,
            TimeSpan.FromMinutes(request.ServiceDurationMinutes));
        Console.WriteLine($"WorkingDay: {workingDay != null}");
        //Console.WriteLine($"Occupied count: {occupied.Count}");
        Console.WriteLine($"Generated slots: {slots.Count}");
        return new AvailabilityResponse(
            request.BarberId,
            request.Date,
            slots);
    }

    // 🔥 CORE SLOT ALGORITHM
    private static IReadOnlyList<AvailabilitySlot> GenerateSlots(
        TimeRange workingRange,
        IReadOnlyList<TimeRange> appointments,
        IReadOnlyList<TimeRange> blocked,
        TimeSpan slotDuration
       )
    {
        var result = new List<AvailabilitySlot>();

        var cursor = workingRange.Start;

        while (cursor + slotDuration <= workingRange.End)
        {
            var slot = new TimeRange(cursor, cursor + slotDuration);

            var overlaps = appointments.Any(a => a.Overlaps(slot));

            var blockedSlot = blocked.Any(b => b.Overlaps(slot));

            if (!overlaps && !blockedSlot)
            {
                result.Add(new AvailabilitySlot(
                    slot.Start,
                    slot.End,isAvailable: true
                    ));
            }

            cursor = cursor.Add(slotDuration);
        }

        return result;
    }
}


