using FluentAssertions;
using HypeSoft.Application.Behaviors;
using HypeSoft.Application.Interfaces;
using HypeSoft.Application.Products.Commands;
using HypeSoft.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Caching;

public class CacheInvalidationBehaviorTests
{
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<CacheInvalidationBehavior<CreateProductCommand, ProductDto>>> _loggerMock;
    private readonly CacheInvalidationBehavior<CreateProductCommand, ProductDto> _behavior;

    public CacheInvalidationBehaviorTests()
    {
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<CacheInvalidationBehavior<CreateProductCommand, ProductDto>>>();
        _behavior = new CacheInvalidationBehavior<CreateProductCommand, ProductDto>(
            _cacheServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenProductCommand_ShouldInvalidateProductCaches()
    {
        // Arrange
        var command = new CreateProductCommand(new CreateProductDto("Test", "Desc", 10m, "cat1", 5));
        var response = new ProductDto("1", "SKU001", "Test", "Desc", 10m, "cat1", "Category", 5, false, DateTime.UtcNow, DateTime.UtcNow);

        RequestHandlerDelegate<ProductDto> next = () => Task.FromResult(response);

        // Act
        var result = await _behavior.Handle(command, next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        _cacheServiceMock.Verify(x => x.RemoveByPrefixAsync("GetAllProductsQuery:", It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(x => x.RemoveByPrefixAsync("GetProductByIdQuery:", It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(x => x.RemoveByPrefixAsync("GetLowStockProductsQuery:", It.IsAny<CancellationToken>()), Times.Once);
        _cacheServiceMock.Verify(x => x.RemoveByPrefixAsync("GetDashboardQuery:", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCacheInvalidationFails_ShouldNotThrowAndReturnResponse()
    {
        // Arrange
        var command = new CreateProductCommand(new CreateProductDto("Test", "Desc", 10m, "cat1", 5));
        var response = new ProductDto("1", "SKU001", "Test", "Desc", 10m, "cat1", "Category", 5, false, DateTime.UtcNow, DateTime.UtcNow);

        _cacheServiceMock
            .Setup(x => x.RemoveByPrefixAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Cache error"));

        RequestHandlerDelegate<ProductDto> next = () => Task.FromResult(response);

        // Act
        var result = await _behavior.Handle(command, next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
    }
}
