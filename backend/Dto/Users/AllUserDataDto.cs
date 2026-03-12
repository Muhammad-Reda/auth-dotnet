using backend.Models;

namespace backend.Dto.Users;

public record AllUserDataDto
(
    Guid Id,
    string Email,
    string Username,
    string PasswordHash,
    User.UserRole Role,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime DeletedAt
);