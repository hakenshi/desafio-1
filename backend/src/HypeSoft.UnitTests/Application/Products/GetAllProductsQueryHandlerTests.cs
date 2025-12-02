using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Products.Queries;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Products;

public class GetAllProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllProductsQueryHandler _handler;

    public GetAllProductsQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllProductsQueryHandler(
            _productRepositoryMock.Object, 
            _categoryRepositoryMock.Object, 
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithoutCategoryFilter_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("Product 1", "Description 1", 10.0m, "cat-1", 100),
            Product.Create("Product 2", "Description 2", 20.0m, "cat-2", 200),
        };

        var categories = new List<Category>
        {
            Category.Create("Category 1", "Description 1"),
            Category.Create("Category 2", "Description 2"),
        };
        // Set IDs to match product category IDs
        typeof(Category).GetProperty("Id")!.SetValue(categories[0], "cat-1");
        typeof(Category).GetProperty("Id")!.SetValue(categories[1], "cat-2");

        _productRepositoryMock
            .Setup(x => x.GetAllAsync(1, 10, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        _productRepositoryMock
            .Setup(x => x.GetTotalCountAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var query = new GetAllProductsQuery(1, 10, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WithCategoryFilter_ReturnsFilteredProducts()
    {
        // Arrange
        var categoryId = "cat-1";
        var products = new List<Product>
        {
            Product.Create("Product 1", "Description 1", 10.0m, categoryId, 100),
        };

        var categories = new List<Category>
        {
            Category.Create("Category 1", "Description 1"),
        };
        typeof(Category).GetProperty("Id")!.SetValue(categories[0], categoryId);

        _productRepositoryMock
            .Setup(x => x.GetAllAsync(1, 10, categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        _productRepositoryMock
            .Setup(x => x.GetTotalCountAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var query = new GetAllProductsQuery(1, 10, categoryId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(1);
        
        _productRepositoryMock.Verify(
            x => x.GetAllAsync(1, 10, categoryId, It.IsAny<CancellationToken>()), 
            Times.Once);
        _productRepositoryMock.Verify(
            x => x.GetTotalCountAsync(categoryId, It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var products = new List<Product>
        {
            Product.Create("Product 1", "Description 1", 10.0m, "cat-1", 100),
        };

        var categories = new List<Category>
        {
            Category.Create("Category 1", "Description 1"),
        };
        typeof(Category).GetProperty("Id")!.SetValue(categories[0], "cat-1");

        _productRepositoryMock
            .Setup(x => x.GetAllAsync(2, 5, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        _productRepositoryMock
            .Setup(x => x.GetTotalCountAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);
        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var query = new GetAllProductsQuery(2, 5, null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalCount.Should().Be(10);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task Handle_EmptyResult_ReturnsEmptyList()
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetAllAsync(1, 10, "non-existent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());
        _productRepositoryMock
            .Setup(x => x.GetTotalCountAsync("non-existent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _categoryRepositoryMock
            .Setup(x => x.GetByIdsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var query = new GetAllProductsQuery(1, 10, "non-existent");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }
}
