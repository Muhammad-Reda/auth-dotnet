using Microsoft.EntityFrameworkCore;

namespace backend.Models;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Username), IsUnique = true)]
public class User
{
    public enum UserRole
    {
        SuperAdmin,
        Admin,
        User
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    required public string Email { get; set; }
    required public string Username { get; set; }
    required public string PasswordHash { get; set; }
    required public UserRole Role { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public Profile? Profile { get; set; }
    public ICollection<RefreshToken>? RefreshToken { get; set; }
}
