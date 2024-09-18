using Domain.Common.Interfaces;
using MediatR;
using Domain.User;
using Application.Interfaces;
using Domain.Common.Implementations;

namespace Application.Commands.Users.UploadDriverLicenseImage;

public class UploadDriverLicenseImageRequestHandler : IRequestHandler<UploadDriverLicenseImageRequest, IResult<UploadDriverLicenseImageResponse>>
{
    private readonly IImageUploader _imageUploader;
    private readonly IUserProfileRepository _userProfileRepository;

    public UploadDriverLicenseImageRequestHandler(IImageUploader imageUploader, IUserProfileRepository userProfileRepository)
    {
        _imageUploader = imageUploader;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<IResult<UploadDriverLicenseImageResponse>> Handle(UploadDriverLicenseImageRequest request, CancellationToken cancellationToken)
    {
        List<string> errors = new();

        if (!Guid.TryParse(request.UserId, out var userId))
        {
            errors.Add("Invalid UserId format.");
        }

        var uploadResult = await _imageUploader.UploadImageAsync(request.DriverLicenseImage);
        if (!uploadResult.IsSuccess)
        {
            errors.AddRange(uploadResult.Errors);
        }

        IResult<UserProfile?> getUserProfileResult = await _userProfileRepository.GetByUserIdAsync(userId);
        if (!getUserProfileResult.IsSuccess)
        {
            errors.AddRange(getUserProfileResult.Errors);
        }

        UserProfile updatedUserProfile = getUserProfileResult.Entity;

        if (!string.IsNullOrEmpty(updatedUserProfile.DriverLicenseImageLocation))
        {
            var deleteOldImageResult = await _imageUploader.DeleteImageAsync(updatedUserProfile.DriverLicenseImageLocation);
            if (!deleteOldImageResult.IsSuccess)
            {
                errors.AddRange(deleteOldImageResult.Errors);
            }
        }

        if (errors.Any())
        {
            return Result<UploadDriverLicenseImageResponse>.Fail(errors);
        }

        updatedUserProfile.DriverLicenseImageLocation = uploadResult.Entity;

        IResult<bool> updateUserProfileResult = await _userProfileRepository.UpdateAsync(updatedUserProfile);

        if (!updateUserProfileResult.IsSuccess)
        {
            return Result<UploadDriverLicenseImageResponse>.Fail(
                new List<string> { "Error while updatding the user profile" }
            );
        }

        var response = new UploadDriverLicenseImageResponse
        {
            UserId = updatedUserProfile.UserId,
            ImageLocation = uploadResult.Entity
        };

        return Result<UploadDriverLicenseImageResponse>.Success(response);
    }
}