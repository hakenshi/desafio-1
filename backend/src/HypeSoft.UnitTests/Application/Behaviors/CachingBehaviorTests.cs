using FluentAssertions;
using HypeSoft.Application.Behaviors;
using HypeSoft.Application.Interfaces;
using HypeSoft.Application.Products.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace HypeSoft.UnitTests.Application.Behaviors;

public class CachingBehaviorTests
{
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<CachingBehavior<GetAllProductsQuery, IEnumerable<object>>>> _loggerMock;
    private readonly CachingBehavior<GetAllProductsQuery, IEnumerable<object>> _behavior;

    public CachingBehaviorTests()
    {
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<CachingBehavior<GetAllProductsQuery, IEnumerable<object>>>>();
        _behavior = new CachingBehavior<GetAllProductsQuery, IEnumerable<object>>(_cacheServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCacheHit_ShouldReturnCachedData()
    {
        // Arrange
        var query = new GetAllProductsQuery(1, 10);
        var cachedData = new List<object> { new { Id = 1, Name = "Cached Product" } };
        
        _cacheServiceMock
            .Setup(x => x.GetAsync<IEnumerable<object>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedData);

        var nextCalled = false;
        Task<IEnumerable<object>> Next()
        {
            nextCalled = true;
            return Task.FromResult<IEnumerable<object>>(new List<object>());
        }

        // Act
        var result = await _behavior.Handle(query, Next, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(cachedData);
        nextCalled.Should().BeFalse();
        _cacheServiceMock.Verify(
            x => x.GetAsync<IEnumerable<object>>(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCacheMiss_ShouldExecuteQueryAndCacheResult()
    {
        // Arrange
        var query = new GetAllProductsQuery(1, 10);
        var freshData = new List<object> { new { Id = 2, Name = "Fresh Product" } };
        
        _cacheServiceMock
            .Setup(x => x.GetAsync<IEnumerable<object>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<object>?)null);

        _cacheServiceMock
            .Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<IEnumerable<object>>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var nextCalled = false;
        Task<IEnumerable<object>> Next()
        {
            nextCalled = true;
            return Task.FromResult<IEnumerable<object>>(freshData);
        }

        // Act
        var result = await _behavior.Handle(query, Next, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(freshData);
        nextCalled.Should().BeTrue();
        
        _cacheServiceMock.Verify(
            x => x.GetAsync<IEnumerable<object>>(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
        
        _cacheServiceMock.Verify(
            x => x.SetAsync(It.IsAny<string>(), freshData, It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCacheThrowsException_ShouldContinueWithoutCache()
    {
        // Arrange
        var query = new GetAllProductsQuery(1, 10);
        var freshData = new List<object> { new { Id = 3, Name = "Product" } };
        
        _cacheServiceMock
            .Setup(x => x.GetAsync<IEnumerable<object>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Redis connection failed"));

        Task<IEnumerable<object>> Next() => Task.FromResult<IEnumerable<object>>(freshData);

        // Act
        var result = await _behavior.Handle(query, Next, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(freshData);
    }
}
