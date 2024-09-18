using System.Text.Json.Serialization;
using Domain.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Users.UploadDriverLicenseImage;

public class UploadDriverLicenseImageRequest : IRequest<IResult<UploadDriverLicenseImageResponse>>
{
    public string UserId { get; set; }
    public IFormFile? DriverLicenseImage { get; set; }
}
