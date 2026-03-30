using BarberBooking.Application.Services.Commands;
using BarberBooking.Application.Services.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BarberBooking.API.Controllers
{
    [ApiController]
    [Route("api/services/{barberId:guid}")]
    public class ServicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServicesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid barberId)
        {
            var services = await _mediator.Send(
                new GetBarberServicesQuery(barberId));

            return Ok(services);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Guid barberId,
            CreateServiceCommand command)
        {
            var id = await _mediator.Send(
                command with { BarberId = barberId });
           
            if (barberId != command.BarberId)
                return BadRequest("BarberId mismatch");

            return CreatedAtAction(nameof(Get), new { barberId }, id);
        }
    }
}
