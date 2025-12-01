using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Dashboard.Queries;

public class GetRecentProductsQueryHandler : IRequestHandler<GetRecentProductsQuery, IEnumerable<RecentProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetRecentProductsQueryHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<RecentProductDto>> Handle(GetRecentProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetRecentAsync(request.Count, cancellationToken);
        var productList = products.ToList();
        
        var categoryIds = productList.Select(p => p.CategoryId).Distinct().ToList();
        var categories = await _categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        var categoryDict = categories.ToDictionary(c => c.Id, c => c.Name);

        return productList.Select(p => new RecentProductDto(
            p.Id,
            p.Name,
            p.Price,
            categoryDict.GetValueOrDefault(p.CategoryId, "Unknown"),
            p.StockQuantity,
            p.CreatedAt
        ));
    }
}
