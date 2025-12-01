using HypeSoft.Domain.Entities;

namespace HypeSoft.Domain.Repositories;

public interface IAuditLogRepository
{
    Task<AuditLog> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditLog>> GetRecentAsync(int count = 10, CancellationToken cancellationToken = default);
}
