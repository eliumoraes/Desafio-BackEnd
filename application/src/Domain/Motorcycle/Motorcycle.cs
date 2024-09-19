using System.Text.RegularExpressions;
using Domain.Common.Implementations;
using Domain.Common.Interfaces;

namespace Domain.Motorcycle;

public class Motorcycle
{
    public Guid MotorcyleId {get; private set;}
    public int Year {get; private set;}
    public string Model {get; private set;}
    public string LicensePlate {get; private set;}

    private List<string> _errors = new();

    public Motorcycle(Guid motorcyleid, int year, string model, string licensePlate)
    {
        MotorcyleId = motorcyleid;
        SetYear(year);
        Model = model;
        SetLicensePlate(licensePlate);
    }

    public Motorcycle(int year, string model, string licensePlate)
    {
        MotorcyleId = Guid.NewGuid();
        SetYear(year);
        Model = model;
        SetLicensePlate(licensePlate);
    }

    private void SetYear(int year)
    {
        if(year <= 1800 || year > DateTime.UtcNow.Year){
            _errors.Add("Invalid year");
        }
    }

    private void SetLicensePlate(string licensePlate)
    {
        licensePlate = CleanMask(licensePlate).Trim().ToUpper();
        if(String.IsNullOrWhiteSpace(licensePlate))
        {
            _errors.Add("The License plate can't be empty");
        }

        if(licensePlate.Length != 7)
        {
            _errors.Add("The License plate must have 7 characters");
        }

        LicensePlate = licensePlate;
    }

    public IResult<Motorcycle> Validate()
    {
        return _errors.Any()
            ? Result<Motorcycle>.Fail(_errors, this)
            : Result<Motorcycle>.Success(this);
    }

    private string CleanMask(string maskedValue)
    {
        return Regex.Replace(maskedValue, "[^a-zA-Z0-9]", "");
    }
}