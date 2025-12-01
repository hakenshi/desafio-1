using AutoMapper;
using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Products.Queries;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public SearchProductsQueryHandler(
        IProductRepository productRepository, 
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.SearchByNameAsync(request.Name, cancellationToken);
        var productList = products.ToList();
        
        var categoryIds = productList.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await _categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        var categoryDict = categories.ToDictionary(c => c.Id, c => c.Name);

        return productList.Select(p => new ProductDto(
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
    }
}
