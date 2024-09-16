namespace Application.Commands.Users.RegisterDeliveryDriver;

public class RegisterDeliveryDriverResponse
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string? UserImageUrl { get; set; }
}