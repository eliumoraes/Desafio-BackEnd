using Domain.Common.Interfaces;
using MediatR;
using Domain.User;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Common.Implementations;

namespace Application.Commands.Users.RegisterDeliveryDriver;

public class RegisterDeliveryDriverRequestHandler : IRequestHandler<RegisterDeliveryDriverRequest, IResult<RegisterDeliveryDriverResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IPasswordHasher _passwordHassher;
    private readonly IImageUploader _imageUploader;

    public RegisterDeliveryDriverRequestHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IPasswordHasher passwordHasher,
        IImageUploader imageUploader)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _passwordHassher = passwordHasher;
        _imageUploader = imageUploader;
    }

    public async Task<IResult<RegisterDeliveryDriverResponse>> Handle(RegisterDeliveryDriverRequest request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            return Result<RegisterDeliveryDriverResponse>.Fail(
                new List<string> { "username already exists" }
            );
        }

        var passwordHash = _passwordHassher.HashPassword(request.Password);

        var user = new User(request.Username, passwordHash, UserRole.DeliveryDriver);
        var userValidationResult = user.Validate();
        if (!userValidationResult.IsSuccess)
        {
            return Result<RegisterDeliveryDriverResponse>.Fail(
                userValidationResult.Errors.ToList()
            );
        }

        var existingProfile = await _userProfileRepository.GetByBusinesseIdentificationNumberOrDriverLicenseAsync(request.BusinessIdentificationNumber, request.DriverLicenseNumber);
        if (existingProfile != null)
        {
            return Result<RegisterDeliveryDriverResponse>.Fail(
                new List<string> { "A profile with the same CNPJ or CNH already exists" }
            );
        }

        string driverLicenseImageUrl = null;
        if (request.DriverLicenseImage != null)
        {
            var imageUploadResult = await _imageUploader.UploadImageAsync(request.DriverLicenseImage);
            if(!imageUploadResult.IsSuccess)
            {
                return Result<RegisterDeliveryDriverResponse>.Fail(
                    imageUploadResult.Errors.ToList()
                );
            }
            driverLicenseImageUrl = imageUploadResult.Entity;
        }

        var userProfile = new UserProfile(
            user,
            request.Name,
            request.BusinessIdentificationNumber,
            request.BirthDate,
            request.DriverLicenseNumber,
            request.DriverLicenseTypes,
            driverLicenseImageUrl
        );

        var profileValidationResult = userProfile.Validate();
        if(!profileValidationResult.IsSuccess)
        {
            return Result<RegisterDeliveryDriverResponse>.Fail(
                profileValidationResult.Errors.ToList()
            );
        }

        var userAdded = await _userRepository.AddAsync(user);
        if(!userAdded){
            return Result<RegisterDeliveryDriverResponse>.Fail(
                new List<string> { "Failed to add user "}
            );
        }

        var profileAdded = await _userProfileRepository.AddAsync(userProfile);
        if(!profileAdded){
            return Result<RegisterDeliveryDriverResponse>.Fail(
                new List<string> { "Failed to add user profile"}
            );
        }

        var response = new RegisterDeliveryDriverResponse
        {
            UserId = user.Id,
            Username = user.Username
        };

        return Result<RegisterDeliveryDriverResponse>.Success(response);

    }

}