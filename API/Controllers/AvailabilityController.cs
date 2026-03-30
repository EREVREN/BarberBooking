using BarberBooking.Application.Availability.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace BarberBooking.API.Controllers;

[ApiController]
[Route("api/availability")]
public sealed class AvailabilityController : ControllerBase
{
    private readonly IMediator _mediator;

    public AvailabilityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns available time slots for a barber on a given date
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] Guid barberId,
        [FromQuery] DateTime date,
        [FromQuery] int serviceDurationMinutes = 30)
    {
        if (barberId == Guid.Empty)
            return BadRequest("barberId is required");

        if (date == default)
            return BadRequest("date is required");

        var query = new GetAvailabilityQuery(
            barberId,
            date,
            serviceDurationMinutes);

        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
