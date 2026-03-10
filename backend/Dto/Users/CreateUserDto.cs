using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Dto.Users;

public record CreateUserDto
(
    [Required][EmailAddress] string Email,
    [StringLength(255, MinimumLength = 2)] string Username,
    [StringLength(255, MinimumLength = 8)] string Password,
    // Need enum (SuperAdmin || Admin || User)
    [Required][EnumDataType(typeof(User.UserRole), ErrorMessage = "Role is invalid")] User.UserRole Role
);
