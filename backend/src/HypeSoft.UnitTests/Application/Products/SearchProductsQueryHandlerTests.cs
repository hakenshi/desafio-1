using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.Products.Queries;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Products;

public class SearchProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly SearchProductsQueryHandler _handler;

    public SearchProductsQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new SearchProductsQueryHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_SearchByName_ReturnsMatchingProducts()
    {
        // Arrange
        var categoryId = "cat-1";
        var products = new List<Product>
        {
            Product.Create("Notebook Dell", "Description 1", 1000.0m, categoryId, 50),
            Product.Create("Notebook HP", "Description 2", 900.0m, categoryId, 30),
        };
        typeof(Product).GetProperty("Id")!.SetValue(products[0], "prod-1");
        typeof(Product).GetProperty("Id")!.SetValue(products[1], "prod-2");

        var category = Category.Create("Electronics", "Electronic products");
        typeof(Category).GetProperty("Id")!.SetValue(category, categoryId);

        _productRepositoryMock
            .Setup(x => x.SearchByNameAsync("Notebook", It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category> { category });

        var query = new SearchProductsQuery("Notebook");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.All(p => p.Name.Contains("Notebook")).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NoMatches_ReturnsEmptyList()
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.SearchByNameAsync("NonExistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new SearchProductsQuery("NonExistent");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CategoryNotFound_UsesUnknownCategoryName()
    {
        // Arrange
        var categoryId = "cat-1";
        var products = new List<Product>
        {
            Product.Create("Test Product", "Description", 100.0m, categoryId, 50),
        };
        typeof(Product).GetProperty("Id")!.SetValue(products[0], "prod-1");

        _productRepositoryMock
            .Setup(x => x.SearchByNameAsync("Test", It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new SearchProductsQuery("Test");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.First().CategoryName.Should().Be("Unknown");
    }
}
