namespace backend.Models;

public class Post
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Content { get; set; } = null!;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int UpVote { get; set; }
    public int DownVote { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DeletedAt { get; set; }
    public required Guid ProfileId { get; set; }
    public ICollection<Profile>? Profile { get; set; }

}
