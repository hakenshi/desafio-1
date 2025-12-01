using HypeSoft.Application.Interfaces;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;

namespace HypeSoft.Infraestructure.Services;

public class AuditService : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task LogAsync(
        string userId,
        string username,
        string action,
        string entityType,
        string entityId,
        string? entityName = null,
        string? details = null,
        CancellationToken cancellationToken = default)
    {
        var log = AuditLog.Create(userId, username, action, entityType, entityId, entityName, details);
        await _auditLogRepository.CreateAsync(log, cancellationToken);
    }
}
