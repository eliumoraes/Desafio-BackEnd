using Application.Interfaces;
using Domain.User;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Common.Interfaces;
using Domain.Common.Implementations;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IResult<bool>> AddAsync(User user)
    {
        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            //TODO: Log error
            Console.WriteLine(ex.Message);
            return Result<bool>.Fail(new List<string> { "Failed to add user" });
        }
    }

    public async Task<IResult<User?>> GetByIdAsync(Guid id)
    {
        try
        {
            User? result = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if(result == null)
            {
                return Result<User?>.Fail(new List<string> { "User not found" });
            }
            return Result<User?>.Success(result);
        }
        catch (Exception)
        {
            return Result<User?>.Fail(new List<string> { "Failed to get user" });
        }
    }

    public async Task<IResult<User?>> GetByUsernameAsync(string username)
    {
        try
        {
            User? result = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == username);
            if(result == null)
            {
                return Result<User?>.Fail(new List<string> { "User not found" });
            }
            return Result<User?>.Success(result);
        }
        catch (Exception)
        {
            return Result<User?>.Fail(new List<string> { "Failed to get user" });
        }
    }
}