using Application.Interfaces.Repositories;
using BarberBooking.Application.Interfaces.Repositories;

using BarberBooking.Application.Models;
using BarberBooking.Domain.Entities;


namespace BarberBooking.Application.Services
{
    public class SlotAvailabilityService
    {
        private readonly IAppointmentRepository _appointments;
        private readonly IWorkingDayRepository _workingDays;
        private readonly IBlockedSlotRepository _blockedSlots;

        public SlotAvailabilityService(
            IAppointmentRepository appointments,
            IWorkingDayRepository workingDays,
            IBlockedSlotRepository blockedSlots)
        {
            _appointments = appointments;
            _workingDays = workingDays;
            _blockedSlots = blockedSlots;
        }

        public async Task<List<AvailableSlot>> GetAvailableSlots(
            Guid barberId,
            DateTime date,
            int serviceDurationMinutes)
        {
            var workingDay = await _workingDays
                .GetForBarberAndDay(barberId, date.DayOfWeek);

            if (workingDay == null)
                return new();

            var dayStart = date.Date + workingDay.StartTime;
            var dayEnd = date.Date + workingDay.EndTime;

            var appointments = await _appointments
                .GetForBarberAndDate(barberId, date);

            var blocked = await _blockedSlots
                .GetForBarberAndDate(barberId, date);

            var duration = TimeSpan.FromMinutes(serviceDurationMinutes);
            var slots = new List<AvailableSlot>();

            for (var start = dayStart;
                 start + duration <= dayEnd;
                 start = start.AddMinutes(15))
            {
                var end = start + duration;

                if (IsOverlapping(start, end, appointments, blocked))
                    continue;

                slots.Add(new AvailableSlot
                {
                    StartTime = start,
                    EndTime = end
                });
            }
        

            return slots;
        }
        
        

        private static bool IsOverlapping(
            DateTime start,
            DateTime end,
            IEnumerable<Appointment> appointments,
            IEnumerable<BlockedSlot> blockedSlots)
        {
            return appointments.Any(a =>
                       start < a.EndTime && a.StartTime < end)
                || blockedSlots.Any(b =>
                       start < b.EndTime && b.StartTime < end);
        }

    }
}
