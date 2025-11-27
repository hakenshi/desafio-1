using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Dashboard.Queries;

public record GetDashboardQuery : IRequest<DashboardDto>;
