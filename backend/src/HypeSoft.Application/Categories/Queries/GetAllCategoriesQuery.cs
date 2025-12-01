using HypeSoft.Application.DTOs;
using MediatR;

namespace HypeSoft.Application.Categories.Queries;

public record GetAllCategoriesQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedResponse<CategoryDto>>;
