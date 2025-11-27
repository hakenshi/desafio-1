using MediatR;

namespace HypeSoft.Application.Products.Commands;

public record DeleteProductCommand(string Id) : IRequest<Unit>;
