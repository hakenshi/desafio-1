using FluentAssertions;
using HypeSoft.Application.Dashboard.Queries;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Dashboard;

public class GetRecentProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly GetRecentProductsQueryHandler _handler;

    public GetRecentProductsQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new GetRecentProductsQueryHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsRecentProducts()
    {
        var categoryId = "cat-1";
        var products = new List<Product>
        {
            Product.Create("Product 1", "Description 1", 99.99m, categoryId, 100),
            Product.Create("Product 2", "Description 2", 49.99m, categoryId, 50),
        };
        typeof(Product).GetProperty("Id")!.SetValue(products[0], "prod-1");
        typeof(Product).GetProperty("Id")!.SetValue(products[1], "prod-2");

        var category = Category.Create("Electronics", "Electronic products");
        typeof(Category).GetProperty("Id")!.SetValue(category, categoryId);

        _productRepositoryMock
            .Setup(x => x.GetRecentAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category> { category });

        var query = new GetRecentProductsQuery(10);
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Product 1");
        result.First().CategoryName.Should().Be("Electronics");
    }

    [Fact]
    public async Task Handle_EmptyProducts_ReturnsEmptyList()
    {
        _productRepositoryMock
            .Setup(x => x.GetRecentAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new GetRecentProductsQuery(10);
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
            Product.Create("Product 1", "Description 1", 99.99m, categoryId, 100),
        };
        typeof(Product).GetProperty("Id")!.SetValue(products[0], "prod-1");

        _productRepositoryMock
            .Setup(x => x.GetRecentAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new GetRecentProductsQuery(10);
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.First().CategoryName.Should().Be("Unknown");
    }

    [Fact]
    public async Task Handle_CustomCount_UsesCorrectCount()
    {
        _productRepositoryMock
            .Setup(x => x.GetRecentAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new GetRecentProductsQuery(5);
        await _handler.Handle(query, CancellationToken.None);
        _productRepositoryMock.Verify(x => x.GetRecentAsync(5, It.IsAny<CancellationToken>()), Times.Once);
    }
}
