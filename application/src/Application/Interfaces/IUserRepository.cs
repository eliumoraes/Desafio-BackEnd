using Domain.Common.Interfaces;
using Domain.User;
namespace Application.Interfaces;

public interface IUserRepository
{
    Task<IResult<User?>> GetByUsernameAsync(string username);
    Task<IResult<User?>> GetByIdAsync(Guid id);
    Task<IResult<bool>> AddAsync(User user);
}