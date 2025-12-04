using AutoMapper;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Products.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IAuditService auditService,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _auditService = auditService;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {request.Id} not found");

        product.Update(
            request.Product.Name,
            request.Product.Description,
            request.Product.Price,
            request.Product.CategoryId,
            request.Product.StockQuantity
        );

        await _productRepository.UpdateAsync(product, cancellationToken);

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken);
        var categoryName = category?.Name ?? "Unknown";
        await _auditService.LogAsync(
            _currentUser.UserId ?? "system",
            _currentUser.Username ?? "system",
            "Update",
            "Product",
            product.Id,
            product.Name,
            $"Updated product",
            cancellationToken
        );

        return new ProductDto(
            product.Id,
            product.Sku,
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
