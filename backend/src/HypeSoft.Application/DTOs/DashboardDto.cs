namespace HypeSoft.Application.DTOs;

public record DashboardDto(
    int TotalProducts,
    decimal TotalStockValue,
    int LowStockCount,
    Dictionary<string, int> ProductsByCategory
);
