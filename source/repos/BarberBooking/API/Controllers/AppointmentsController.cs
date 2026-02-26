using BarberBooking.Application.Appointments.Queries;
using BarberBooking.Application.Appointments.Commands;
using BarberBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace BarberBooking.API.Controllers;

[ApiController]
[Route("api/appointments")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var appointment = await _mediator.Send(
            new GetAppointmentByIdQuery(id));

        if (appointment is null)
            return NotFound();

        return Ok(appointment);
    }
 
    [HttpGet("barber/{barberId:guid}")]
    public async Task<IActionResult> GetForBarber(
        Guid barberId,
        [FromQuery] DateTime date)
    {
        var result = await _mediator.Send(
            new GetAppointmentsForBarberQuery(barberId, date));

        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
    [FromQuery] AppointmentFilter filter,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(
            new GetAppointmentsPagedQuery(filter, page, pageSize));

        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> Get(
    [FromQuery] Guid? barberId,
    [FromQuery] Guid? serviceId,
    [FromQuery] Guid? customerId,
    [FromQuery] AppointmentStatus? status,
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
    {
        var filter = new AppointmentFilter
        {
            BarberId = barberId,
            ServiceId = serviceId,
            CustomerId = customerId,
            Status = status,
            From = from,
            To = to
        };

        var result = await _mediator.Send(
            new GetAppointmentsQuery(filter, page, pageSize));

        return Ok(result);
    }
    // POST api/appointments

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm([FromBody] ConfirmAppointmentCommand command) {

        var id = await _mediator.Send(command);
        return Ok(new { id });
    }
     
}