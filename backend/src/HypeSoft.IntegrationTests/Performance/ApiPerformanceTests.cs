using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HypeSoft.Application.DTOs;

namespace HypeSoft.IntegrationTests.Performance;

public class ApiPerformanceTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private const int MaxResponseTimeMs = 500;

    public ApiPerformanceTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_ShouldRespondInLessThan500ms()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync("/api/products?page=1&pageSize=10");
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs, 
            $"API should respond in less than {MaxResponseTimeMs}ms, but took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task GetAllProducts_SecondCall_ShouldBeFasterDueToCache()
    {
        // Arrange - First call to populate cache
        await _client.GetAsync("/api/products?page=1&pageSize=10");

        // Act - Second call should hit cache
        var stopwatch = Stopwatch.StartNew();
        var response = await _client.GetAsync("/api/products?page=1&pageSize=10");
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100, 
            "Cached response should be very fast (< 100ms)");
    }

    [Fact]
    public async Task GetDashboard_ShouldRespondInLessThan500ms()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync("/api/dashboard");
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs,
            $"Dashboard should respond in less than {MaxResponseTimeMs}ms, but took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task GetCategories_ShouldRespondInLessThan500ms()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync("/api/categories");
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs,
            $"Categories should respond in less than {MaxResponseTimeMs}ms, but took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task SearchProducts_ShouldRespondInLessThan500ms()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync("/api/products/search?name=Notebook");
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs,
            $"Search should respond in less than {MaxResponseTimeMs}ms, but took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task GetLowStockProducts_ShouldRespondInLessThan500ms()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync("/api/products/low-stock");
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs,
            $"Low stock query should respond in less than {MaxResponseTimeMs}ms, but took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task CreateProduct_ShouldRespondInLessThan500ms()
    {
        // Arrange
        var categoryId = await GetOrCreateCategoryId();
        var newProduct = new CreateProductDto(
            "Performance Test Product",
            "Testing response time for product creation",
            99.99m,
            categoryId,
            50
        );

        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", newProduct);
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs,
            $"Product creation should respond in less than {MaxResponseTimeMs}ms, but took {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task MultipleSequentialRequests_ShouldAllRespondInLessThan500ms()
    {
        // Arrange
        var requests = new[]
        {
            "/api/products?page=1&pageSize=10",
            "/api/categories",
            "/api/dashboard",
            "/api/products/low-stock"
        };

        // Act & Assert
        foreach (var endpoint in requests)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await _client.GetAsync(endpoint);
            stopwatch.Stop();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(MaxResponseTimeMs,
                $"Endpoint {endpoint} should respond in less than {MaxResponseTimeMs}ms, but took {stopwatch.ElapsedMilliseconds}ms");
        }
    }

    [Fact]
    public async Task ConcurrentRequests_ShouldAllRespondInLessThan500ms()
    {
        // Arrange
        var tasks = Enumerable.Range(0, 5).Select(async i =>
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await _client.GetAsync("/api/products?page=1&pageSize=10");
            stopwatch.Stop();

            return new
            {
                Response = response,
                ElapsedMs = stopwatch.ElapsedMilliseconds
            };
        });

        // Act
        var results = await Task.WhenAll(tasks);

        // Assert
        foreach (var result in results)
        {
            result.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.ElapsedMs.Should().BeLessThan(MaxResponseTimeMs,
                $"Concurrent request should respond in less than {MaxResponseTimeMs}ms, but took {result.ElapsedMs}ms");
        }
    }

    private async Task<string> GetOrCreateCategoryId()
    {
        var categoriesResponse = await _client.GetAsync("/api/categories");
        var categories = await categoriesResponse.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>();
        
        if (categories?.Any() == true)
        {
            return categories.First().Id;
        }

        var newCategory = new CreateCategoryDto("Performance Test Category", "Test Description");
        var response = await _client.PostAsJsonAsync("/api/categories", newCategory);
        var created = await response.Content.ReadFromJsonAsync<CategoryDto>();
        return created!.Id;
    }
}
