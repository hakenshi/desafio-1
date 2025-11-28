using FluentAssertions;
using HypeSoft.Application.Interfaces;
using HypeSoft.Infraestructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;
using System.Text.Json;

namespace HypeSoft.UnitTests.Infrastructure.Caching;

public class RedisCacheServiceTests
{
    private readonly Mock<IDistributedCache> _cacheMock;
    private readonly RedisCacheService _cacheService;

    public RedisCacheServiceTests()
    {
        _cacheMock = new Mock<IDistributedCache>();
        _cacheService = new RedisCacheService(_cacheMock.Object);
    }

    [Fact]
    public async Task GetAsync_WhenDataExists_ShouldReturnDeserializedObject()
    {
        // Arrange
        var testData = new TestCacheData { Id = 1, Name = "Test" };
        var serializedData = JsonSerializer.Serialize(testData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var bytes = Encoding.UTF8.GetBytes(serializedData);

        _cacheMock
            .Setup(x => x.GetAsync("test-key", It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        // Act
        var result = await _cacheService.GetAsync<TestCacheData>("test-key");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetAsync_WhenDataDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _cacheMock
            .Setup(x => x.GetAsync("non-existing-key", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        // Act
        var result = await _cacheService.GetAsync<TestCacheData>("non-existing-key");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_ShouldSerializeAndStoreData()
    {
        // Arrange
        var testData = new TestCacheData { Id = 2, Name = "Cache Test" };
        string? capturedData = null;

        _cacheMock
            .Setup(x => x.SetAsync(
                "test-key",
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (key, value, options, ct) => capturedData = Encoding.UTF8.GetString(value))
            .Returns(Task.CompletedTask);

        // Act
        await _cacheService.SetAsync("test-key", testData);

        // Assert
        capturedData.Should().NotBeNull();
        capturedData.Should().Contain("\"id\":2");
        capturedData.Should().Contain("\"name\":\"Cache Test\"");
        
        _cacheMock.Verify(
            x => x.SetAsync(
                "test-key",
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetAsync_WithCustomExpiration_ShouldUseProvidedExpiration()
    {
        // Arrange
        var testData = new TestCacheData { Id = 3, Name = "Expiration Test" };
        var customExpiration = TimeSpan.FromMinutes(10);
        DistributedCacheEntryOptions? capturedOptions = null;

        _cacheMock
            .Setup(x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (key, value, options, ct) => capturedOptions = options)
            .Returns(Task.CompletedTask);

        // Act
        await _cacheService.SetAsync("test-key", testData, customExpiration);

        // Assert
        capturedOptions.Should().NotBeNull();
        capturedOptions!.AbsoluteExpirationRelativeToNow.Should().Be(customExpiration);
    }

    [Fact]
    public async Task SetAsync_WithoutCustomExpiration_ShouldUseDefaultExpiration()
    {
        // Arrange
        var testData = new TestCacheData { Id = 4, Name = "Default Expiration" };
        DistributedCacheEntryOptions? capturedOptions = null;

        _cacheMock
            .Setup(x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (key, value, options, ct) => capturedOptions = options)
            .Returns(Task.CompletedTask);

        // Act
        await _cacheService.SetAsync("test-key", testData);

        // Assert
        capturedOptions.Should().NotBeNull();
        capturedOptions!.AbsoluteExpirationRelativeToNow.Should().Be(TimeSpan.FromMinutes(5));
    }

    [Fact]
    public async Task RemoveAsync_ShouldCallCacheRemove()
    {
        // Arrange
        _cacheMock
            .Setup(x => x.RemoveAsync("test-key", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _cacheService.RemoveAsync("test-key");

        // Assert
        _cacheMock.Verify(
            x => x.RemoveAsync("test-key", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private class TestCacheData
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
