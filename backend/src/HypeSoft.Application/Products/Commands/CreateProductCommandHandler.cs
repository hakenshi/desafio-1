using AutoMapper;
using HypeSoft.Application.DTOs;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using MediatR;

namespace HypeSoft.Application.Products.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Product.Name,
            Description = request.Product.Description,
            Price = request.Product.Price,
            CategoryId = request.Product.CategoryId,
            StockQuantity = request.Product.StockQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _productRepository.CreateAsync(product, cancellationToken);
        return _mapper.Map<ProductDto>(created);
    }
}
