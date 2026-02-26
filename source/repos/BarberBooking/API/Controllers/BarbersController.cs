using BarberBooking.Application.Barbers.Commands;
using BarberBooking.Application.Barbers.Queries;
using BarberBooking.Application.Services.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BarberBooking.API.Controllers;

[ApiController]
[Route("api/barbers")]
public class BarbersController : ControllerBase
{
    private readonly IMediator _mediator;
   

    public BarbersController(IMediator mediator)
    {
        _mediator = mediator;
      
    }
    // GET api/barbers
   
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var barbers = await _mediator.Send(new GetBarbersQuery());
        return Ok(barbers);
    }
  
    // GET api/barbers/{barberId}/services
   
  [HttpGet("{barberId:guid}/services")]
    public async Task<IActionResult> GetServices(Guid barberId)
    {
        var result = await _mediator.Send(
            new GetBarberServicesQuery(barberId)
        );

        return Ok(result);
    } 
   
}