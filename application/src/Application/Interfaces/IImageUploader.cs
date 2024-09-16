using Domain.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IImageUploader
{
    Task<IResult<string>> UploadImageAsync(IFormFile image);
}