using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Auth;

public record RegisterDto
(
    [Required][EmailAddress] string Email,
    [StringLength(255, MinimumLength = 2)] string Username,
    [StringLength(255, MinimumLength = 8)] string Password
);