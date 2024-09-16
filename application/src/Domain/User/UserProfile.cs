using System.Text.RegularExpressions;
using DocumentValidator;
using Domain.Common.Implementations;
using Domain.Common.Interfaces;

namespace Domain.User;

public class UserProfile
{
    public Guid UserProfileId { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string ProfileType { get; set; } = "DeliveryDriver"; // DeliveryDriver (por enquanto só temos um, mas poderíamos usar um enumerator pra outros profiles, exemplo accountant, etc)
    public string Name { get; set; }
    public string BusinessIdentificationNumber { get; set; } // CNPJ
    public DateTime? DateOfBirth { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public HashSet<DriverLicenseType>? DriverLicenseType { get; set; }
    public string? DriverLicenseImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    private List<string> _errors = new();

    public UserProfile()
    {        
    }

    public UserProfile(User user, string name, string businessIdentificationNumber, DateTime? dateOfBirth, string? driverLicenseNumber, IEnumerable<string> driverLicenseTypes, string? driverLicenseImageUrl)
    {
        UserId = user.Id;
        User = user;
        SetName(name);
        SetBusinessIdentificationNumber(businessIdentificationNumber);
        SetDateOfBirth(dateOfBirth);
        SetDriverLicenseNumber(driverLicenseNumber);
        SetDriverLicenseType(driverLicenseTypes);
        DriverLicenseImageUrl = driverLicenseImageUrl;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _errors.Add("Name is required");
            return;
        }

        Name = name;
    }

    public void SetBusinessIdentificationNumber(string businessIdentificationNumber)
    {
        if (string.IsNullOrWhiteSpace(businessIdentificationNumber))
        {
            _errors.Add("Business Identification Number is required");
            return;
        }

        if(!CnpjValidation.Validate(businessIdentificationNumber))
        {
            _errors.Add("Invalid Business Identification Number, try using 'https://www.4devs.com.br/gerador_de_cnpj'");
            return;
        }

        BusinessIdentificationNumber = CleanMask(businessIdentificationNumber);
    }

    public void SetDateOfBirth(DateTime? dateOfBirth)
    {
        if (dateOfBirth == null)
        {
            _errors.Add("Date of Birth is required");
            return;
        }

        DateOfBirth = dateOfBirth;
    }

    public void SetDriverLicenseNumber(string? driverLicenseNumber)
    {
        if (string.IsNullOrWhiteSpace(driverLicenseNumber))
        {
            _errors.Add("Driver License Number is required");
            return;
        }

        if(!CnhValidation.Validate(driverLicenseNumber))
        {
            _errors.Add("Invalid Driver License Number, try using 'https://www.4devs.com.br/gerador_de_cnh'");
            return;
        }

        DriverLicenseNumber = CleanMask(driverLicenseNumber);
    }

    public void SetDriverLicenseType(IEnumerable<string> driverLicenseTypes)
    {
        if (driverLicenseTypes == null || !driverLicenseTypes.Any())
        {
            _errors.Add("Driver License Type is required");
            return;
        }

        DriverLicenseType = new HashSet<DriverLicenseType>();

        foreach (var driverLicenseType in driverLicenseTypes)
        {
            if (Enum.TryParse<DriverLicenseType>(driverLicenseType, out var result))
            {
                DriverLicenseType.Add(result);
            }
            else
            {
                _errors.Add($"Invalid Driver License Type: {driverLicenseType}");
                return;
            }
        }
    }

    private string CleanMask(string maskedValue)
    {
        return Regex.Replace(maskedValue, "[^a-zA-Z0-9]", "");
    }

    public IResult<UserProfile> Validate()
    {
        return _errors.Any() 
            ? Result<UserProfile>.Fail(_errors, this) 
            : Result<UserProfile>.Success(this);
    }
}
