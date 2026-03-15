using backend.Models;

namespace backend.Dto.Users;

public record UserDto
(
    Guid Id,
    string Email,
    string Username,
    string Password,
    User.UserRole Role,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime DeletedAt
);