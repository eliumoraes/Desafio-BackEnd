namespace Application.Interfaces;

using Domain.Common.Interfaces;
using Domain.User;

public interface IUserProfileRepository
{
    Task<IResult<UserProfile?>> GetByBusinesseIdentificationNumberOrDriverLicenseAsync(string? businessIdentificationNumber, string? driverLicenseNumber);
    Task<IResult<UserProfile?>> GetByUserIdAsync(Guid userId);
    Task<IResult<bool>> AddAsync(UserProfile userProfile);
    Task<IResult<bool>> UpdateAsync(UserProfile userProfile);

}