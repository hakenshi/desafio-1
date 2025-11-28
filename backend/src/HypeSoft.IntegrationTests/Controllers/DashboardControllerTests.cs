using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HypeSoft.Application.DTOs;

namespace HypeSoft.IntegrationTests.Controllers;

public class DashboardControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DashboardControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ShouldReturnDashboardData()
    {
        // Act
        var response = await _client.GetAsync("/api/dashboard");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dashboard = await response.Content.ReadFromJsonAsync<DashboardDto>();
        
        dashboard.Should().NotBeNull();
        dashboard!.TotalProducts.Should().BeGreaterOrEqualTo(0);
        dashboard.TotalStockValue.Should().BeGreaterOrEqualTo(0);
        dashboard.LowStockCount.Should().BeGreaterOrEqualTo(0);
        dashboard.ProductsByCategory.Should().NotBeNull();
    }

    [Fact]
    public async Task Get_AfterCreatingProducts_ShouldReflectChanges()
    {
        // Arrange - Get initial state
        var initialResponse = await _client.GetAsync("/api/dashboard");
        var initialDashboard = await initialResponse.Content.ReadFromJsonAsync<DashboardDto>();

        // Create a category
        var category = new CreateCategoryDto("Dashboard Test Category", "Test category description");
        var categoryResponse = await _client.PostAsJsonAsync("/api/categories", category);
        var createdCategory = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();

        // Create a product
        var product = new CreateProductDto("Dashboard Product", "Test product description for dashboard", 100m, createdCategory!.Id, 50);
        await _client.PostAsJsonAsync("/api/products", product);

        // Act - Get updated state
        var updatedResponse = await _client.GetAsync("/api/dashboard");
        var updatedDashboard = await updatedResponse.Content.ReadFromJsonAsync<DashboardDto>();

        // Assert
        updatedDashboard.Should().NotBeNull();
        updatedDashboard!.TotalProducts.Should().BeGreaterThan(initialDashboard!.TotalProducts);
        updatedDashboard.TotalStockValue.Should().BeGreaterThan(initialDashboard.TotalStockValue);
    }
}
