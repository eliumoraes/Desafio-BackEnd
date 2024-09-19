using Microsoft.AspNetCore.Http;
using FluentValidation;

namespace Application.Commands.Users.UploadDriverLicenseImage;

public class UploadDriverLicenseImageValidator : AbstractValidator<UploadDriverLicenseImageRequest>
{
    public UploadDriverLicenseImageValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(x => x.DriverLicenseImage)
            .NotNull().WithMessage("Image is required.")
            .Must(BeValidImageType).When(x => x.DriverLicenseImage != null)
            .WithMessage("Only PNG and BMP images are allowed.")
            .Must(BeValidImageSize).When(x => x.DriverLicenseImage != null)
            .WithMessage("Image size must be less than 5 MB.");
    }

    // Funções utilitárias de validação
    private bool BeValidImageType(IFormFile file) => new[] { "image/png", "image/bmp" }.Contains(file.ContentType);
    private bool BeValidImageSize(IFormFile file) => file.Length <= (5 * 1024 * 1024); // 5 MB
}
