using AutoMapper;
using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Categories.Queries;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, PaginatedResponse<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var totalCount = await _categoryRepository.GetTotalCountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PaginatedResponse<CategoryDto>(
            Items: _mapper.Map<IEnumerable<CategoryDto>>(categories),
            Page: request.Page,
            PageSize: request.PageSize,
            TotalCount: totalCount,
            TotalPages: totalPages
        );
    }
}
