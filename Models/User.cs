
public class User
{
    public int Id { get; set; }

    public required string Username { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public required string Role { get; set; }

    public string? ResetCode { get; set; }

    public DateTime? ResetCodeExpiration { get; set; }

    public List<TravelRequest> TravelRequests { get; set; } = new();
}
