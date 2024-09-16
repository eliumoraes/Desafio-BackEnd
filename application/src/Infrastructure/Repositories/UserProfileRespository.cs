using Application.Interfaces;
using Domain.User;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserProfileRespository : IUserProfileRepository
{
    private readonly AppDbContext _context;
    public UserProfileRespository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<UserProfile?> GetByBusinesseIdentificationNumberOrDriverLicenseAsync(string? businessIdentificationNumber, string? driverLicenseNumber)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(up =>
            up.BusinessIdentificationNumber == businessIdentificationNumber ||
            up.DriverLicenseNumber == driverLicenseNumber);
    }

    public async Task<bool> AddAsync(UserProfile userProfile)
    {
        try
        {
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            //TODO: Log error
            Console.WriteLine(ex.Message);
            return false;
        }
    }
}