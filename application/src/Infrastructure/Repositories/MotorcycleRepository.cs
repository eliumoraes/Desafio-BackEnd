using Application.Interfaces;
using Domain.Common.Implementations;
using Domain.Common.Interfaces;
using Domain.Motorcycle;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MotorcycleRepository : IMotorcycleRepository
{
    private readonly AppDbContext _context;
    public MotorcycleRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IResult<bool>> AddAsync(Motorcycle motorcycle)
    {
        try
        {
            await _context.Motorcycles.AddAsync(motorcycle);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception)
        {
            //Log error (seq, appinsights, etc)
            return Result<bool>.Fail(
                new List<string> {"An error ocurred while trying to add the motorcyle"}
            );
        }
    }

    public Task<IResult<bool>> DeleteAsync(Guid motorcycleId)
    {
        throw new NotImplementedException();
    }

    public Task<IResult<List<Motorcycle>>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IResult<Motorcycle>> GetByLicensePlateAsync(string licensePlate)
    {
        try
        {
            Motorcycle? result = await _context.Motorcycles.FirstOrDefaultAsync(m => m.LicensePlate == licensePlate);
            if(result == null){
                return Result<Motorcycle>.Fail(new List<string> {"Motorcycle not found"});
            }
            return Result<Motorcycle>.Success(result);
        }
        catch (Exception)
        {
            return Result<Motorcycle>.Fail(new List<string> { "Failed to get Motorcycle" });
        }
    }

    public Task<IResult<bool>> UpdateAsync(Motorcycle motorcycle)
    {
        throw new NotImplementedException();
    }
}