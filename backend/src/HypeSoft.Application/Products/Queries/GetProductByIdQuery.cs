using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Products.Queries;

public record GetProductByIdQuery(string Id) : IRequest<ProductDto?>;
