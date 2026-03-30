using BarberBooking.Application.Admin.Calendar.Queries;
using BarberBooking.Application.Appointments.Queries;
using BarberBooking.Application.Barbers.Commands;
using BarberBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace BarberBooking.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // POST api/admin/barbers
        [HttpPost("barbers")]
        public async Task<IActionResult> Create([FromBody] CreateBarberCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { id });
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAdminAppointments(
   [FromQuery] Guid? barberId,
   [FromQuery] AppointmentStatus? status,
   [FromQuery] DateTime? from,
   [FromQuery] DateTime? to,
   [FromQuery] int page = 1,
   [FromQuery] int pageSize = 20)
        {
            var filter = new AppointmentFilter
            {
                BarberId = barberId,
                Status = status,
                From = from,
                To = to
            };

            var result = await _mediator.Send(
                new GetAdminAppointmentsQuery(filter, page, pageSize));

            return Ok(result);
        }
        [HttpGet("calendar/day")]
        public async Task<IActionResult> Day(
            [FromQuery] DateTime date,
            [FromQuery] Guid? barberId)
        {
          
            if (date == default)
                return BadRequest("date is required");
            if (barberId == Guid.Empty)
                return BadRequest("barberId is required");

            return Ok(await _mediator.Send(
                new GetAdminDayTimelineQuery(date, barberId)));
        }

        [HttpGet("calendar/week")]
        public async Task<IActionResult> Week(
            [FromQuery] DateTime weekStart,
            [FromQuery] Guid? barberId)
        {
            if (weekStart == default)
                return BadRequest("barberId is required");
            if (barberId == Guid.Empty)
                return BadRequest("barberId is required");

            return Ok(await _mediator.Send(
                new GetAdminWeekTimelineQuery(weekStart, barberId)));
        }
    }

}
