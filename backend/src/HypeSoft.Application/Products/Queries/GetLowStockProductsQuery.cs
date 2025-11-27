using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Products.Queries;

public record GetLowStockProductsQuery : IRequest<IEnumerable<ProductDto>>;
