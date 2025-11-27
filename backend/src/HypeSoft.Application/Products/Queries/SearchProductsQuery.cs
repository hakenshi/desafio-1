using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Products.Queries;

public record SearchProductsQuery(string Name) : IRequest<IEnumerable<ProductDto>>;
