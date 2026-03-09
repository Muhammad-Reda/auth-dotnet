using backend.Models;

namespace backend.Dto.Users;

public record UserDetailsDto
(
    Guid Id,
    string Email,
    string Username,
    User.UserRole Role,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime DeletedAt
);