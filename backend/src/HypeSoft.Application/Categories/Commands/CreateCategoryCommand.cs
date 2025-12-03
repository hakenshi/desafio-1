using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Categories.Commands;

public record CreateCategoryCommand(CreateCategoryDto Category) : IRequest<CategoryDto>;
