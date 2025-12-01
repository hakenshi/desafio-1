using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Dashboard.Queries;

public record GetRecentProductsQuery(int Count = 10) : IRequest<IEnumerable<RecentProductDto>>;
