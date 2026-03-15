using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Auth;

public record LoginDto
(
    [Required] string Username,
    [Required] string Password
);