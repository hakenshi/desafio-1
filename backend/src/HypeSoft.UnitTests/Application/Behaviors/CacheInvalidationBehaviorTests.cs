using FluentAssertions;
using HypeSoft.Application.Behaviors;
using HypeSoft.Application.Products.Commands;
using HypeSoft.Application.DTOs;
using HypeSoft.Infraestructure.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace HypeSoft.UnitTests.Application.Behaviors;

public class CacheInvalidationBehaviorTests
{
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<CacheInvalidationBehavior<CreateProductCommand, ProductDto>>> _loggerMock;
    private readonly CacheInvalidationBehavior<CreateProductCommand, ProductDto> _behavior;

    public CacheInvalidationBehaviorTests()
    {
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<CacheInvalidationBehavior<CreateProductCommand, ProductDto>>>();
        _behavior = new CacheInvalidationBehavior<CreateProductCommand, ProductDto>(_cacheServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenProductCommand_ShouldInvalidateProductCaches()
    {
        // Arrange
        var command = new CreateProductCommand(
            new CreateProductDto("Test", "Description here", 10m, "cat1", 5));
        
        var response = new ProductDto("1", "Test", "Description here", 10m, "cat1", 5, false, DateTime.UtcNow, DateTime.UtcNow);

        _cacheServiceMock
            .Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        Task<ProductDto> Next() => Task.FromResult(response);

        // Act
        var result = await _behavior.Handle(command, Next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        
        _cacheServiceMock.Verify(
            x => x.RemoveAsync(It.Is<string>(s => s.Contains("GetAllProductsQuery")), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
        
        _cacheServiceMock.Verify(
            x => x.RemoveAsync(It.Is<string>(s => s.Contains("GetDashboardQuery")), It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_WhenCacheInvalidationFails_ShouldStillReturnResponse()
    {
        // Arrange
        var command = new CreateProductCommand(
            new CreateProductDto("Test", "Description here", 10m, "cat1", 5));
        
        var response = new ProductDto("1", "Test", "Description here", 10m, "cat1", 5, false, DateTime.UtcNow, DateTime.UtcNow);

        _cacheServiceMock
            .Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Cache service unavailable"));

        Task<ProductDto> Next() => Task.FromResult(response);

        // Act
        var result = await _behavior.Handle(command, Next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
    }
}
