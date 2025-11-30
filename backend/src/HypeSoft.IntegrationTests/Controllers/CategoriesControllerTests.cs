using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HypeSoft.Application.DTOs;

namespace HypeSoft.IntegrationTests.Controllers;

public class CategoriesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/categories");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/categories/test-id");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var category = new CreateCategoryDto("Test Category", "Test Description");

        // Act
        var response = await _client.PostAsJsonAsync("/api/categories", category);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
