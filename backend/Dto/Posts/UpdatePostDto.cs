using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Posts;

public record UpdatePostDto
(
    [StringLength(maximumLength: 500, MinimumLength = 1)] string Content
);