using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Dashboard.Queries;

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetDashboardQueryHandler(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var totalProducts = await _productRepository.GetTotalCountAsync(cancellationToken);
        var totalStockValue = await _productRepository.GetTotalStockValueAsync(cancellationToken);
        var lowStockProducts = await _productRepository.GetLowStockProductsAsync(cancellationToken);
        var productsByCategory = await _categoryRepository.GetProductCountByCategoryAsync(cancellationToken);

        return new DashboardDto(
            totalProducts,
            totalStockValue,
            lowStockProducts.Count(),
            productsByCategory
        );
    }
}
