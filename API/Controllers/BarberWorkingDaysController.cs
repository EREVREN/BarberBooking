using BarberBooking.Application.BarberWorkingDays.Queries;
using BarberBooking.Application.BarberWorkingDays.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace BarberBooking.API.Controllers
{
    [ApiController]
    [Route("api/barberworkingdays")]
    public class BarberWorkingDaysController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BarberWorkingDaysController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("day")]
        public async Task<IActionResult> GetForBarberAndDay([FromQuery] Guid barberId, 
                                                            [FromQuery] DayOfWeek dayOfWeek)
        {
            var result = await _mediator.Send(new GetForBarberAndDayQuery(barberId, dayOfWeek));
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetForBarberAndDate([FromQuery] Guid barberId,
                                                             [FromQuery] DateTime date)
        {
            var result = await _mediator.Send(new GetForBarberAndDateQuery(barberId, date));
            if (result is null) return NotFound();
            return Ok(result);
        }

        [HttpGet("date")]
        public async Task<IActionResult> GetForBarberAndDateLegacy([FromQuery] Guid barberId,
                                                                    [FromQuery] DateTime date)
        {
            var result = await _mediator.Send(new GetForBarberAndDateQuery(barberId, date));
            if (result is null) return NotFound();
            return Ok(result);
        }
    
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBarberWorkingDayCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
