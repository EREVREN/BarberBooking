using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberBooking.Application.Admin.Calendar.DTOs
{
    public sealed record AdminWeekTimelineDto(
    DateTime WeekStart,
    DateTime WeekEnd,
    IReadOnlyList<AdminCalendarAppointmentDto> Appointments
);
}
