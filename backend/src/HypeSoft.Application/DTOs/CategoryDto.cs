namespace HypeSoft.Application.DTOs;

public record CategoryDto(
    string Id,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateCategoryDto(
    string Name,
    string Description
);

public record UpdateCategoryDto(
    string Name,
    string Description
);
