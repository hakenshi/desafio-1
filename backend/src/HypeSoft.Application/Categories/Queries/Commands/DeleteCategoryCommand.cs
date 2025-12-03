using MediatR;

namespace HypeSoft.Application.Categories.Commands;

public record DeleteCategoryCommand(string Id) : IRequest<bool>;
