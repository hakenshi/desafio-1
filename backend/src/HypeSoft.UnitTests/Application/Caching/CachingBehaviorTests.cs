using FluentAssertions;
using HypeSoft.Application.Behaviors;
using HypeSoft.Application.Interfaces;
using HypeSoft.Application.Products.Queries;
using HypeSoft.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Caching;

public class CachingBehaviorTests
{
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ILogger<CachingBehavior<GetAllProductsQuery, PaginatedResponse<ProductDto>>>> _loggerMock;
    private readonly CachingBehavior<GetAllProductsQuery, PaginatedResponse<ProductDto>> _behavior;

    public CachingBehaviorTests()
    {
        _cacheServiceMock = new Mock<ICacheService>();
        _loggerMock = new Mock<ILogger<CachingBehavior<GetAllProductsQuery, PaginatedResponse<ProductDto>>>>();
        _behavior = new CachingBehavior<GetAllProductsQuery, PaginatedResponse<ProductDto>>(
            _cacheServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenCacheHit_ShouldReturnCachedResponse()
    {
        // Arrange
        var query = new GetAllProductsQuery(1, 10, null);
        var cachedResponse = new PaginatedResponse<ProductDto>(
            Items: new List<ProductDto>(),
            Page: 1,
            PageSize: 10,
            TotalCount: 0,
            TotalPages: 0
        );

        _cacheServiceMock
            .Setup(x => x.GetAsync<PaginatedResponse<ProductDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedResponse);

        var nextCalled = false;
        RequestHandlerDelegate<PaginatedResponse<ProductDto>> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(cachedResponse);
        };

        // Act
        var result = await _behavior.Handle(query, next, CancellationToken.None);

        // Assert
        result.Should().Be(cachedResponse);
        nextCalled.Should().BeFalse();
        _cacheServiceMock.Verify(x => x.GetAsync<PaginatedResponse<ProductDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCacheMiss_ShouldCallNextAndCacheResponse()
    {
        // Arrange
        var query = new GetAllProductsQuery(1, 10, null);
        var response = new PaginatedResponse<ProductDto>(
            Items: new List<ProductDto>(),
            Page: 1,
            PageSize: 10,
            TotalCount: 0,
            TotalPages: 0
        );

        _cacheServiceMock
            .Setup(x => x.GetAsync<PaginatedResponse<ProductDto>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaginatedResponse<ProductDto>?)null);

        var nextCalled = false;
        RequestHandlerDelegate<PaginatedResponse<ProductDto>> next = () =>
        {
            nextCalled = true;
            return Task.FromResult(response);
        };

        // Act
        var result = await _behavior.Handle(query, next, CancellationToken.None);

        // Assert
        result.Should().Be(response);
        nextCalled.Should().BeTrue();
        _cacheServiceMock.Verify(x => x.SetAsync(It.IsAny<string>(), response, It.IsAny<TimeSpan?>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
