using Domain.Common.Interfaces;
using MediatR;
using Domain.User;
using Application.Interfaces;
using Domain.Common.Implementations;
using System.Globalization;

namespace Application.Commands.Users.RegisterDeliveryDriver;

public class RegisterDeliveryDriverRequestHandler : IRequestHandler<RegisterDeliveryDriverRequest, IResult<RegisterDeliveryDriverResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IPasswordHasher _passwordHassher;

    public RegisterDeliveryDriverRequestHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _passwordHassher = passwordHasher;
    }

    public async Task<IResult<RegisterDeliveryDriverResponse>> Handle(RegisterDeliveryDriverRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        IResult<User> existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser.IsSuccess)
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

        IResult<UserProfile> existingProfile = await _userProfileRepository.GetByBusinesseIdentificationNumberOrDriverLicenseAsync(request.BusinessIdentificationNumber, request.DriverLicenseNumber);
        if (existingProfile.IsSuccess)
        {
            errors.Add("A profile with the same CNPJ or CNH already exists");
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
            null
        );

        var profileValidationResult = userProfile.Validate();
        if (!profileValidationResult.IsSuccess)
        {
            errors.AddRange(profileValidationResult.Errors);
        }        

        // Entity, application or service errors
        if (errors.Any())
        {
            return Result<RegisterDeliveryDriverResponse>.Fail(errors);
        }

        IResult<bool> userAdded = await _userRepository.AddAsync(user);
        if (!userAdded.IsSuccess)
        {
            errors.AddRange(userAdded.Errors);
        }

        var profileAdded = await _userProfileRepository.AddAsync(userProfile);
        if (!profileAdded.IsSuccess)
        {            
            errors.AddRange(profileAdded.Errors);
        }

        // Repository errors
        if (errors.Any())
        {
            return Result<RegisterDeliveryDriverResponse>.Fail(errors);
        }

        var response = new RegisterDeliveryDriverResponse
        {
            UserId = user.Id,
            Username = user.Username
        };

        return Result<RegisterDeliveryDriverResponse>.Success(response);

    }

}