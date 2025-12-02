using AutoMapper;
using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Products.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository, 
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null) return null;

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken);
        var categoryName = category?.Name ?? "Unknown";

        return new ProductDto(
            product.Id,
            string.IsNullOrEmpty(product.Sku) ? $"PRD{product.Id[..6].ToUpper()}" : product.Sku,
            product.Name,
            product.Description,
            product.Price,
            product.CategoryId,
            categoryName,
            product.StockQuantity,
            product.IsLowStock(),
            product.CreatedAt,
            product.UpdatedAt
        );
    }
}
