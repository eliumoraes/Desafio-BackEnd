using Domain.User;

namespace Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateJwtToken(User user);
}