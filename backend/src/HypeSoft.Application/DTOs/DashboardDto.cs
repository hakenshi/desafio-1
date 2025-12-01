namespace HypeSoft.Application.DTOs;

public record DashboardDto(
    int TotalProducts,
    decimal TotalStockValue,
    int LowStockCount,
    Dictionary<string, int> ProductsByCategory
);

public record AuditLogDto(
    string Id,
    string UserId,
    string Username,
    string Action,
    string EntityType,
    string EntityId,
    string? EntityName,
    string? Details,
    DateTime CreatedAt
);

public record RecentProductDto(
    string Id,
    string Name,
    decimal Price,
    string CategoryName,
    int StockQuantity,
    DateTime CreatedAt
);
