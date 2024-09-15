using Domain.Common.Implementations;
using Domain.Common.Interfaces;

namespace Domain.User;

public class User
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private List<string> _errors = new();

    public User(string username, string passwordHash, UserRole role)
    {
        Id = Guid.NewGuid();
        SetUsername(username);
        SetPasswordHash(passwordHash);
        Role = role;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public User(Guid id, string username, string passwordHash, UserRole role, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;        
    }

    private void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            _errors.Add("Username is required");
            return;
        }

        Username = username;
    }

    private void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            _errors.Add("Password is required");
            return;
        }

        PasswordHash = passwordHash.Trim();
    }

    public IResult Validate()
    {
        return _errors.Any() ? Result.Fail(_errors) : Result.Success();
    }
}