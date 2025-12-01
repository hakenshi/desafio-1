using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Dashboard.Queries;

public record GetRecentAuditLogsQuery(int Count = 10) : IRequest<IEnumerable<AuditLogDto>>;
