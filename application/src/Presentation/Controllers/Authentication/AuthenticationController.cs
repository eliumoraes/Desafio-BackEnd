using Application.Commands.Users.UserAuthentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Authentication;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(UserAuthenticationResponse), 200)]
    [ProducesResponseType(typeof(IEnumerable<string>), 400)]
    public async Task<IActionResult> Authenticate([FromBody] UserAuthenticationRequest request)
    {
        var result = await _mediator.Send(request);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Entity);
    }
}