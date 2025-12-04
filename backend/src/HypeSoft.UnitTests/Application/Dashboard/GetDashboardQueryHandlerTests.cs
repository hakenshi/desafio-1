using FluentAssertions;
using HypeSoft.Application.Dashboard.Queries;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Dashboard;

public class GetDashboardQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly GetDashboardQueryHandler _handler;

    public GetDashboardQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new GetDashboardQueryHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsDashboardData()
    {
        var lowStockProducts = new List<Product>
        {
            Product.Create("Product 1", "Description", 10.0m, "cat-1", 5),
            Product.Create("Product 2", "Description", 20.0m, "cat-1", 3),
        };

        var productsByCategory = new Dictionary<string, int>
        {
            { "Electronics", 50 },
            { "Clothing", 30 },
        };

        _productRepositoryMock
            .Setup(x => x.GetTotalCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);

        _productRepositoryMock
            .Setup(x => x.GetTotalStockValueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(50000m);

        _productRepositoryMock
            .Setup(x => x.GetLowStockProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(lowStockProducts);

        _categoryRepositoryMock
            .Setup(x => x.GetProductCountByCategoryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(productsByCategory);

        var query = new GetDashboardQuery();
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.TotalProducts.Should().Be(100);
        result.TotalStockValue.Should().Be(50000m);
        result.LowStockCount.Should().Be(2);
        result.ProductsByCategory.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_EmptyData_ReturnsZeroValues()
    {
        _productRepositoryMock
            .Setup(x => x.GetTotalCountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _productRepositoryMock
            .Setup(x => x.GetTotalStockValueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        _productRepositoryMock
            .Setup(x => x.GetLowStockProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        _categoryRepositoryMock
            .Setup(x => x.GetProductCountByCategoryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, int>());

        var query = new GetDashboardQuery();
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.TotalProducts.Should().Be(0);
        result.TotalStockValue.Should().Be(0m);
        result.LowStockCount.Should().Be(0);
        result.ProductsByCategory.Should().BeEmpty();
    }
}
