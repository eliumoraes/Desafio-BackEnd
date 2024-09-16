using Domain.Common.Interfaces;
using MediatR;
using Domain.User;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Common.Implementations;
using System.Globalization;

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
        var errors = new List<string>();
        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            errors.Add("Username already exists");
        }

        var passwordHash = _passwordHassher.HashPassword(request.Password);

        var user = new User(request.Username, passwordHash, UserRole.DeliveryDriver);
        var userValidationResult = user.Validate();
        if (!userValidationResult.IsSuccess)
        {
            errors.AddRange(userValidationResult.Errors);
        }

        var existingProfile = await _userProfileRepository.GetByBusinesseIdentificationNumberOrDriverLicenseAsync(request.BusinessIdentificationNumber, request.DriverLicenseNumber);
        if (existingProfile != null)
        {
            errors.Add("A profile with the same CNPJ or CNH already exists");
        }

        string? driverLicenseImageUrl = null;
        if (request.DriverLicenseImage != null)
        {
            var imageUploadResult = await _imageUploader.UploadImageAsync(request.DriverLicenseImage);
            if (!imageUploadResult.IsSuccess)
            {
                errors.AddRange(imageUploadResult.Errors);
            }
            driverLicenseImageUrl = imageUploadResult.Entity;
        }

        if (!DateTime.TryParseExact(request.BirthDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDate))
        {
            errors.Add("Invalid DateBirth format. Please use dd/MM/yyyy.");
        }

        var userProfile = new UserProfile(
            user,
            request.Name,
            request.BusinessIdentificationNumber,
            DateTime.SpecifyKind(birthDate, DateTimeKind.Utc),
            request.DriverLicenseNumber,
            request.DriverLicenseTypes,
            driverLicenseImageUrl
        );

        var profileValidationResult = userProfile.Validate();
        if (!profileValidationResult.IsSuccess)
        {
            errors.AddRange(profileValidationResult.Errors);
        }        

        // Entity, application or service errors
        if (errors.Any())
        {
            if (driverLicenseImageUrl != null)
                await _imageUploader.DeleteImageAsync(driverLicenseImageUrl);
            return Result<RegisterDeliveryDriverResponse>.Fail(errors);
        }

        var userAdded = await _userRepository.AddAsync(user);
        if (!userAdded)
        {
            errors.Add("Failed to add user ");
        }

        var profileAdded = await _userProfileRepository.AddAsync(userProfile);
        if (!profileAdded)
        {            
            errors.Add("Failed to add user profile");
        }

        // Repository errors
        if (errors.Any())
        {
            if (driverLicenseImageUrl != null)
                await _imageUploader.DeleteImageAsync(driverLicenseImageUrl);
            return Result<RegisterDeliveryDriverResponse>.Fail(errors);
        }

        var response = new RegisterDeliveryDriverResponse
        {
            UserId = user.Id,
            Username = user.Username,
            UserImageUrl = userProfile.DriverLicenseImageUrl
        };

        return Result<RegisterDeliveryDriverResponse>.Success(response);

    }

}