using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Categories.Queries;

public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;
