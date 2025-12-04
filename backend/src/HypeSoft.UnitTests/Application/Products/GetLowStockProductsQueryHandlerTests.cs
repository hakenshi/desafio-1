using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.Products.Queries;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Products;

public class GetLowStockProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetLowStockProductsQueryHandler _handler;

    public GetLowStockProductsQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetLowStockProductsQueryHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsLowStockProducts()
    {
        var categoryId = "cat-1";
        var products = new List<Product>
        {
            Product.Create("Product 1", "Description 1", 10.0m, categoryId, 5),
            Product.Create("Product 2", "Description 2", 20.0m, categoryId, 3),
        };
        typeof(Product).GetProperty("Id")!.SetValue(products[0], "prod-1");
        typeof(Product).GetProperty("Id")!.SetValue(products[1], "prod-2");

        var category = Category.Create("Test Category", "Test Description");
        typeof(Category).GetProperty("Id")!.SetValue(category, categoryId);

        _productRepositoryMock
            .Setup(x => x.GetLowStockProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category> { category });

        var query = new GetLowStockProductsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(p => p.IsLowStock).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyList()
    {
        _productRepositoryMock
            .Setup(x => x.GetLowStockProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new GetLowStockProductsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CategoryNotFound_UsesUnknownCategoryName()
    {
        var categoryId = "cat-1";
        var products = new List<Product>
        {
            Product.Create("Product 1", "Description 1", 10.0m, categoryId, 5),
        };
        typeof(Product).GetProperty("Id")!.SetValue(products[0], "prod-1");

        _productRepositoryMock
            .Setup(x => x.GetLowStockProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new GetLowStockProductsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.First().CategoryName.Should().Be("Unknown");
    }
}
