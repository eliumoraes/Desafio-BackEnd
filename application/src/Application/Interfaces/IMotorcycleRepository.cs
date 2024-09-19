using Domain.Common.Interfaces;
using Domain.Common.Implementations;
using Domain.Motorcycle;

namespace Application.Interfaces;

public interface IMotorcycleRepository
{
    Task<IResult<Motorcycle?>> GetByLicensePlateAsync(string licensePlate);
    Task<IResult<List<Motorcycle?>>> GetAllAsync();
    Task<IResult<bool>> AddAsync(Motorcycle motorcycle);
    Task<IResult<bool>> UpdateAsync(Motorcycle motorcycle);
    Task<IResult<bool>> DeleteAsync(Guid motorcycleId);
}