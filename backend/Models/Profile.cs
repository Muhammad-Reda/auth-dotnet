namespace backend.Models;

public class Profile
{
    public Guid Id { get; set; }
    required public string FullName { get; set; }
    required public int Age { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Post>? Post { get; set; }
}
