namespace backend.Models;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    required public string Token { get; set; } = null!;
    required public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    required public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
