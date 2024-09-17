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

        var uploadResult = await _imageUploader.UploadImageAsync(request.DriverLicenseImage);
        if (!uploadResult.IsSuccess)
        {
            errors.AddRange(uploadResult.Errors);
        }

        IResult<UserProfile?> getUserProfileResult = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        if(!getUserProfileResult.IsSuccess)
        {
            errors.AddRange(getUserProfileResult.Errors);
        }

        if(errors.Any()){
            return Result<UploadDriverLicenseImageResponse>.Fail(errors);
        }

        UserProfile updatedUserProfile = getUserProfileResult.Entity;
        updatedUserProfile.DriverLicenseImageUrl = uploadResult.Entity;

        IResult<bool> updateUserProfileResult = await _userProfileRepository.UpdateAsync(updatedUserProfile);

        if(!updateUserProfileResult.IsSuccess){
            return Result<UploadDriverLicenseImageResponse>.Fail(
                new List<string> {"Error while updatding the user profile"}
            );
        }

        var response = new UploadDriverLicenseImageResponse
        {
            UserId = request.UserId,
            ImageUrl = uploadResult.Entity
        };

        return Result<UploadDriverLicenseImageResponse>.Success(response);
    }
}