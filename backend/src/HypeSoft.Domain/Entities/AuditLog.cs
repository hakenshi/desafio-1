namespace HypeSoft.Domain.Entities;

public class AuditLog
{
    public string Id { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
    public string Username { get; private set; } = null!;
    public string Action { get; private set; } = null!;
    public string EntityType { get; private set; } = null!;
    public string EntityId { get; private set; } = null!;
    public string? EntityName { get; private set; }
    public string? Details { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        string userId,
        string username,
        string action,
        string entityType,
        string entityId,
        string? entityName = null,
        string? details = null)
    {
        return new AuditLog
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Username = username,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            EntityName = entityName,
            Details = details,
            CreatedAt = DateTime.UtcNow
        };
    }
}
