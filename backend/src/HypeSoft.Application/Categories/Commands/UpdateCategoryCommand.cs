using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Categories.Commands;

public record UpdateCategoryCommand(string Id, UpdateCategoryDto Category) : IRequest<CategoryDto>;

public record UpdateCategoryDto(string Name, string Description);
