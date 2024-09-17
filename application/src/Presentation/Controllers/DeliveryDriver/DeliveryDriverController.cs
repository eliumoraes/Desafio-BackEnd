using Application.Commands.Users.RegisterDeliveryDriver;
using Domain.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.DeliveryDriver;

[ApiController]
[Route("api/[controller]")]
public class DeliveryDriverController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeliveryDriverController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new delivery driver.
    /// </summary>
    /// <param name="request">Driver details for registration.</param>    
    /// <returns>Created delivery driver details or error.</returns>
    [HttpPost("Register")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RegisterDeliveryDriverResponse), 201)]
    [ProducesResponseType(typeof(IEnumerable<string>), 400)]
    public async Task<IActionResult> RegisterDeliveryDriver([FromBody] RegisterDeliveryDriverRequest request)
    {
        IResult<RegisterDeliveryDriverResponse> result = await _mediator.Send(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var locationUri = $"{Request.Scheme}://{Request.Host}/api/DeliveryDriver/{result.Entity.UserId}";

        return Created(locationUri, result.Entity);
    }
}
