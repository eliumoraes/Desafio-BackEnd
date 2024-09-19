using Application.Commands.Motorcycles.RegisterMotorcycle;
using Domain.Common.Interfaces;
using Domain.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Motorcycle;

public class MotorcycleController : ControllerBase
{
    IMediator _mediator;
    public MotorcycleController(IMediator mediator)
    {
        _mediator = mediator;    
    }

    /// <summary>
    /// Registers a new motorcycle.
    /// </summary>
    /// <param name="request">Motrcycle details for registration</param>
    /// <returns></returns>
    [HttpPost("Register")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RegisterMotorcycleResponse), 201)]
    [ProducesResponseType(typeof(IEnumerable<string>),400)]
    [ProducesResponseType(403)]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> RegisterMotorcycle ([FromBody] RegisterMotorcycleRequest request)
    {
        IResult<RegisterMotorcycleResponse> result = await _mediator.Send(request);

        if(!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var locationUri = $"{Request.Scheme}://{Request.Host}/api/Motorcycle/{result.Entity.MotorcycleId}";

        return Created(locationUri, result.Entity);
    }
}