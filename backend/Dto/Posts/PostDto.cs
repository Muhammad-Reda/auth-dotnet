namespace backend.Dto.Posts;

public record PostDto
(
    Guid Id,
    Guid ProfileId,
    string Content,
    DateTime Date,
    int UpVote,
    int DownVote,
    bool IsDeleted,
    DateTime DeletedAt
);
