using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HypeSoft.Application.DTOs;

namespace HypeSoft.IntegrationTests.Controllers;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/products/invalid-id");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var product = new CreateProductDto(
            "Test Product",
            "Test Description",
            99.99m,
            "category-id",
            10
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Search_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/products/search?name=test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLowStock_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/products/low-stock");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
