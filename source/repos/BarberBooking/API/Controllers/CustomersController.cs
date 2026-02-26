using BarberBooking.Application.Customers.Queries;
using BarberBooking.Application.Customers.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BarberBooking.API.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomersController : ControllerBase
    {

        public IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var customer = await _mediator.Send(new GetCustomerByPhoneQuery(phoneNumber));
                if (customer == null)
                    return NotFound();
                return Ok(customer);
            }
            return BadRequest("Phone number query parameter is required.");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
        {
            var customerId = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { phoneNumber = command.PhoneNumber }, new { Id = customerId });
        }
    }
}
