using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Products.Commands;

public record UpdateProductCommand(string Id, UpdateProductDto Product) : IRequest<ProductDto>;
