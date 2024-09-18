using Application.Commands.Users.UploadDriverLicenseImage;
using Domain.Common.Interfaces;
using Domain.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    [ProducesResponseType(typeof(UploadDriverLicenseImageResponse), 400)]
    [Authorize(Roles = nameof(UserRole.DeliveryDriver))]
    public async Task<IActionResult> UploadDriveLicenseImage(IFormFile driverLicenseImage)
    {
        var request = new UploadDriverLicenseImageRequest();
        request.UserId = User.Claims.FirstOrDefault(c => c.Type == "UserIdentification")?.Value;
        request.DriverLicenseImage = driverLicenseImage;

        IResult<UploadDriverLicenseImageResponse> result = await _mediator.Send(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        var locationUri = result.Entity.ImageLocation;

        return Created(locationUri, result.Entity);
    }
}