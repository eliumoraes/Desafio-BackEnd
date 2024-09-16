namespace Application.Interfaces;
using Domain.User;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByBusinesseIdentificationNumberOrDriverLicenseAsync(string? businessIdentificationNumber, string? driverLicenseNumber);
    Task<bool> AddAsync(UserProfile userProfile);

}