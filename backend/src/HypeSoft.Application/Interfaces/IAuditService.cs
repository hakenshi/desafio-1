namespace HypeSoft.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        string userId,
        string username,
        string action,
        string entityType,
        string entityId,
        string? entityName = null,
        string? details = null,
        CancellationToken cancellationToken = default);
}
