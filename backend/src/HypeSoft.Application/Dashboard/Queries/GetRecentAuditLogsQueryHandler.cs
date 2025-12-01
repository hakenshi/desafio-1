using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Dashboard.Queries;

public class GetRecentAuditLogsQueryHandler : IRequestHandler<GetRecentAuditLogsQuery, IEnumerable<AuditLogDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public GetRecentAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<IEnumerable<AuditLogDto>> Handle(GetRecentAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _auditLogRepository.GetRecentAsync(request.Count, cancellationToken);
        
        return logs.Select(l => new AuditLogDto(
            l.Id,
            l.UserId,
            l.Username,
            l.Action,
            l.EntityType,
            l.EntityId,
            l.EntityName,
            l.Details,
            l.CreatedAt
        ));
    }
}
