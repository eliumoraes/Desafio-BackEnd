using Application.Commands.Users.UploadDriverLicenseImage;
using Domain.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.DeliveryDriver;

[ApiController]
[Route("api/DeliveryDriver/DriveLicenseUpload")]
public class DriveLicenseUploadController : ControllerBase
{
    private readonly IMediator _mediator;

    public DriveLicenseUploadController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Uploads a driver's license image.
    /// </summary>
    /// <param name="request">The driver license image upload.</param>
    /// <returns>Uploaded image URL or error.</returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(IEnumerable<string>), 400)]
    public async Task<IActionResult> UploadDriveLicenseImage([FromForm] UploadDriverLicenseImageRequest request)
    {
        IResult<UploadDriverLicenseImageResponse> result = await _mediator.Send(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var locationUri = result.Entity.ImageUrl;

        return Created(locationUri, result.Entity);
    }
}