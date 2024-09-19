using Domain.Common.Interfaces;
using MediatR;
using FluentValidation;
using Domain.Common.Interfaces;
using Domain.Common.Implementations;
using MediatR;
using Domain.User;
using Application.Interfaces;
using Domain.Common.Implementations;
using System.Globalization;

namespace Application.Commands.Users.UserAuthentication;

public class UserAuthenticationRequest : IRequest<IResult<UserAuthenticationResponse>>
{
    public string UserName { get; set; }
    public string Password { get; set; }

}

public class UserAuthenticationResponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Token { get; set; }
}

public class UserAuthenticationRequestValidator : AbstractValidator<UserAuthenticationRequest>
{
    public UserAuthenticationRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}

public class UserAuthenticationRequestHandler : IRequestHandler<UserAuthenticationRequest, IResult<UserAuthenticationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public UserAuthenticationRequestHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<IResult<UserAuthenticationResponse>> Handle(UserAuthenticationRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        IResult<User?> user = await _userRepository.GetByUsernameAsync(request.UserName);

        if (!user.IsSuccess)
        {
            errors.AddRange(user.Errors);
        }

        if (errors.Any())
        {
            return Result<UserAuthenticationResponse>.Fail(errors);
        }

        bool userPasswordMatchResult = _passwordHasher.VerifyPassword(request.Password, user.Entity.PasswordHash);

        if (!userPasswordMatchResult)
        {
            errors.Add("Invalid password");
        }

        if (errors.Any())
        {
            return Result<UserAuthenticationResponse>.Fail(errors);
        }

        string token = _jwtTokenService.GenerateJwtToken(user.Entity);

        return Result<UserAuthenticationResponse>.Success(
            new UserAuthenticationResponse
            {
                UserId = user.Entity.Id,
                UserName = user.Entity.Username,
                Token = token
            });
    }
}