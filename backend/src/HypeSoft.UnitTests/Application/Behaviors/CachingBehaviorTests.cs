using FluentAssertions;
using HypeSoft.Application.Behaviors;
using HypeSoft.Application.Interfaces;
using HypeSoft.Application.Products.Queries;
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
    public async Task Handle_ShouldExecuteNextDelegate()
    {
        // Arrange
        var query = new GetAllProductsQuery(1, 10);
        var expectedData = new List<object> { new { Id = 1, Name = "Product" } };
        
        var nextCalled = false;
        Task<IEnumerable<object>> Next()
        {
            nextCalled = true;
            return Task.FromResult<IEnumerable<object>>(expectedData);
        }

        // Act
        var result = await _behavior.Handle(query, Next, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(expectedData);
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnResultFromNextDelegate()
    {
        // Arrange
        var query = new GetAllProductsQuery(1, 10);
        var freshData = new List<object> { new { Id = 2, Name = "Fresh Product" } };
        
        Task<IEnumerable<object>> Next() => Task.FromResult<IEnumerable<object>>(freshData);

        // Act
        var result = await _behavior.Handle(query, Next, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(freshData);
    }

    [Fact]
    public async Task Handle_WhenNextThrowsException_ShouldPropagateException()
    {
        // Arrange
        var query = new GetAllProductsQuery(1, 10);
        
        Task<IEnumerable<object>> Next() => throw new InvalidOperationException("Test exception");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _behavior.Handle(query, Next, CancellationToken.None));
    }
}
