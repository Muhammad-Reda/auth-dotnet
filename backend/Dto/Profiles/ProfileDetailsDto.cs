namespace backend.Dto.Profiles;

public record ProfileDetailsDto
(
    Guid Id,
    string FullName,
    int Age,
    string Phone,
    string Address,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime DeletedAt
);