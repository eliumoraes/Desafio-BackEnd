using Application.Interfaces;
using Domain.User;
using Microsoft.EntityFrameworkCore;
using Domain.Common.Interfaces;
using Domain.Common.Implementations;

namespace Infrastructure.Repositories;

public class UserProfileRespository : IUserProfileRepository
{
    private readonly AppDbContext _context;
    public UserProfileRespository(AppDbContext context)
    {
        _context = context;

    }
    public async Task<IResult<UserProfile?>> GetByBusinesseIdentificationNumberOrDriverLicenseAsync(string? businessIdentificationNumber, string? driverLicenseNumber)
    {
        try
        {
            UserProfile? result = await _context.UserProfiles
            .FirstOrDefaultAsync(up =>
            up.BusinessIdentificationNumber == businessIdentificationNumber ||
            up.DriverLicenseNumber == driverLicenseNumber);

            if (result == null)
            {
                return Result<UserProfile?>.Fail(new List<string> { "User profile not found" });
            }

            return Result<UserProfile?>.Success(result);
        }
        catch (Exception)
        {
            // TODO: Log error
            return Result<UserProfile?>.Fail(
                new List<string> {
                     "An error occurred while trying to get the user profile"
                     }
                );
        }
    }

    public async Task<IResult<UserProfile?>> GetByUserIdAsync(Guid userId)
    {
        try
        {
            UserProfile? result = await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (result == null)
            {
                return Result<UserProfile?>.Fail(new List<string> { "User profile not found" });
            }
            return Result<UserProfile?>.Success(result);
        }
        catch (Exception ex)
        {
            //TODO: Log error
            return Result<UserProfile?>.Fail(
                new List<string> {
                     "An error occurred while trying to get the user profile"
                     }
                );
        }
    }

    public async Task<IResult<bool>> AddAsync(UserProfile userProfile)
    {
        try
        {
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            //TODO: Log error
            Console.WriteLine(ex.Message);
            return Result<bool>.Fail(
                new List<string> {
                     "An error occurred while trying to add the user profile"
                     }
                );
        }
    }

    public async Task<IResult<bool>> UpdateAsync(UserProfile userProfile)
    {
        try
        {
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception)
        {

            return Result<bool>.Fail(
                new List<string> {
                    "An error ocurred while trying to update the user profile"
                }
            );
        }
    }
}