using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.Dto.Users;

public record CreateUserDto
(
    [Required][EmailAddress] string Email,
    [Required][StringLength(255, MinimumLength = 2)] string Username,
    [Required][StringLength(255, MinimumLength = 8)] string Password,
    [Required][EnumDataType(typeof(User.UserRole), ErrorMessage = "Role is invalid")] User.UserRole? Role
);
