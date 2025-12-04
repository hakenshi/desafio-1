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
        var response = await _client.GetAsync("/api/products");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/products/invalid-id");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithoutAuth_ShouldReturnUnauthorized()
    {
        var product = new CreateProductDto(
            "Test Product",
            "Test Description",
            99.99m,
            "category-id",
            10
        );
        var response = await _client.PostAsJsonAsync("/api/products", product);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Search_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/products/search?name=test");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLowStock_WithoutAuth_ShouldReturnUnauthorized()
    {
        var response = await _client.GetAsync("/api/products/low-stock");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
