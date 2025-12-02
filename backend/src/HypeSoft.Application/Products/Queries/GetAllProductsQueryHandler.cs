using AutoMapper;
using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Products.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginatedResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(
        IProductRepository productRepository, 
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(request.Page, request.PageSize, request.CategoryId, cancellationToken);
        var totalCount = await _productRepository.GetTotalCountAsync(request.CategoryId, cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        // Get all unique category IDs and fetch categories
        var categoryIds = products.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await _categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        var categoryDict = categories.ToDictionary(c => c.Id, c => c.Name);

        var productDtos = products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.CategoryId,
            categoryDict.GetValueOrDefault(p.CategoryId, "Unknown"),
            p.StockQuantity,
            p.IsLowStock(),
            p.CreatedAt,
            p.UpdatedAt
        ));

        return new PaginatedResponse<ProductDto>(
            Items: productDtos,
            Page: request.Page,
            PageSize: request.PageSize,
            TotalCount: totalCount,
            TotalPages: totalPages
        );
    }
}
