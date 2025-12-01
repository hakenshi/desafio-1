using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using HypeSoft.Infraestructure.Data;
using MongoDB.Driver;

namespace HypeSoft.Infraestructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly MongoDbContext _context;

    public AuditLogRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.InsertOneAsync(auditLog, cancellationToken: cancellationToken);
        return auditLog;
    }

    public async Task<IEnumerable<AuditLog>> GetRecentAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _context.AuditLogs
            .Find(_ => true)
            .SortByDescending(a => a.CreatedAt)
            .Limit(count)
            .ToListAsync(cancellationToken);
    }
}
