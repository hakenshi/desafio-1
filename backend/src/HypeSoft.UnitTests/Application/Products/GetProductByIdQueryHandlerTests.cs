using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.Products.Queries;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Products;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetProductByIdQueryHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingProduct_ReturnsProduct()
    {
        // Arrange
        var productId = "prod-1";
        var categoryId = "cat-1";
        var product = Product.Create("Test Product", "Test Description", 99.99m, categoryId, 100);
        typeof(Product).GetProperty("Id")!.SetValue(product, productId);

        var category = Category.Create("Test Category", "Test Description");
        typeof(Category).GetProperty("Id")!.SetValue(category, categoryId);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        var query = new GetProductByIdQuery(productId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Product");
        result.CategoryName.Should().Be("Test Category");
    }

    [Fact]
    public async Task Handle_NonExistingProduct_ReturnsNull()
    {
        // Arrange
        var productId = "non-existent";

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var query = new GetProductByIdQuery(productId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Handle_CategoryNotFound_UsesUnknownCategoryName()
    {
        // Arrange
        var productId = "prod-1";
        var categoryId = "cat-1";
        var product = Product.Create("Test Product", "Test Description", 99.99m, categoryId, 100);
        typeof(Product).GetProperty("Id")!.SetValue(product, productId);

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var query = new GetProductByIdQuery(productId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.CategoryName.Should().Be("Unknown");
    }
}
