using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Products.Queries;

public record GetAllProductsQuery(int Page = 1, int PageSize = 10, string? CategoryId = null) : IRequest<PaginatedResponse<ProductDto>>;
