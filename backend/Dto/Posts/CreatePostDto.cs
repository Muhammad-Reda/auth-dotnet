using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Posts;

public record class CreatePostDto
(
    [StringLength(maximumLength: 500, MinimumLength = 1)] string Content
);
