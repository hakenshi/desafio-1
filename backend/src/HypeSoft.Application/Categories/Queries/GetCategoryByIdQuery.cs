using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Categories.Queries;

public record GetCategoryByIdQuery(string Id) : IRequest<CategoryDto?>;
