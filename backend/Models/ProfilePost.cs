namespace backend.Models;

public class ProfilePost
{
    public Guid ProfileId { get; set; }
    public Guid PostId { get; set; }
    public Profile Profile { get; set; } = null!;
    public Post Post { get; set; } = null!;
}
