using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Auth;

public record RefreshTokenDto
(
    string Token,
    DateTime ExpiresAt,
    bool IsRevoked,
    Guid UserId
);