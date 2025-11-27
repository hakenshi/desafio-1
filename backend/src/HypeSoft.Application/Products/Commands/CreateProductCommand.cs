using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Products.Commands;

public record CreateProductCommand(CreateProductDto Product) : IRequest<ProductDto>;
