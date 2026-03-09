using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Profiles;

public record UpdateProfileDto
(
    [StringLength(255, MinimumLength = 2)] string FullName,
    [Range(13, 150)] int Age,
    [StringLength(500, MinimumLength = 2)] string? Address,
    [StringLength(20, MinimumLength = 9)] string? Phone
);