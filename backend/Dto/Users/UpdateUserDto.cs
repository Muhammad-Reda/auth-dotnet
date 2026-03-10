using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Dto.Users;

public record UpdateUserDto
(
    [Required][EmailAddress] string Email,
    [StringLength(255, MinimumLength = 2)] string Username,
    [Required] User.UserRole Role
);
