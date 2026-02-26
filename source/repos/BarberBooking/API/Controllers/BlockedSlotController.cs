using BarberBooking.Application.BlockedSlots.Commands;
using BarberBooking.Application.BlockedSlots.Queries;
using BarberBooking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BarberBooking.API.Controllers
{
    [ApiController]
    [Route("api/blockedslots")]
    public class BlockedSlotController : ControllerBase
    {
        public readonly IMediator _mediator;
        public BlockedSlotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlockedSlot(
            [FromQuery] Guid barberId,
            [FromQuery] DateTime date)
        {
            if (barberId == Guid.Empty)
                return BadRequest("barberId is required");
            if (date == default)
                return BadRequest("date is required");

            return Ok(await _mediator.Send(
                new GetBlockedSlotQuery(barberId, date)));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBlockedSlot(
            [FromBody] CreateBlockedSlotCommand command)

        {

            return Ok(await _mediator.Send(command));
                
        }
    }
}
