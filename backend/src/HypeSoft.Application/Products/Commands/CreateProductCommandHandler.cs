using AutoMapper;
using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Products.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(
        IProductRepository productRepository, 
        ICategoryRepository categoryRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.Product.Name,
            request.Product.Description,
            request.Product.Price,
            request.Product.CategoryId,
            request.Product.StockQuantity
        );

        var created = await _productRepository.CreateAsync(product, cancellationToken);
        
        var category = await _categoryRepository.GetByIdAsync(created.CategoryId, cancellationToken);
        var categoryName = category?.Name ?? "Unknown";

        return new ProductDto(
            created.Id,
            created.Name,
            created.Description,
            created.Price,
            created.CategoryId,
            categoryName,
            created.StockQuantity,
            created.IsLowStock(),
            created.CreatedAt,
            created.UpdatedAt
        );
    }
}
