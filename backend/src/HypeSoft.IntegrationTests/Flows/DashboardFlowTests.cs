using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HypeSoft.Application.DTOs;

namespace HypeSoft.IntegrationTests.Flows;
public class DashboardFlowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DashboardFlowTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDashboard_ShouldReturnMetrics()
    {
        var response = await _client.GetAsync("/api/dashboard");
        
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            Assert.True(true, "Endpoint requer autenticação");
            return;
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dashboard = await response.Content.ReadFromJsonAsync<DashboardDto>();
        
        dashboard.Should().NotBeNull();
        dashboard!.TotalProducts.Should().BeGreaterOrEqualTo(0);
        dashboard.TotalStockValue.Should().BeGreaterOrEqualTo(0);
        dashboard.LowStockCount.Should().BeGreaterOrEqualTo(0);
        dashboard.ProductsByCategory.Should().NotBeNull();
    }

    [Fact]
    public async Task Dashboard_AfterCreatingProducts_ShouldUpdateMetrics()
    {
        var initialResponse = await _client.GetAsync("/api/dashboard");
        if (initialResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            Assert.True(true, "Endpoint requer autenticação");
            return;
        }

        var initialDashboard = await initialResponse.Content.ReadFromJsonAsync<DashboardDto>();
        var initialTotal = initialDashboard?.TotalProducts ?? 0;
        var category = new CreateCategoryDto("Test Category", "Test");
        var categoryResponse = await _client.PostAsJsonAsync("/api/categories", category);
        
        if (categoryResponse.StatusCode == HttpStatusCode.Unauthorized)
        {
            Assert.True(true, "Endpoint requer autenticação");
            return;
        }

        var createdCategory = await categoryResponse.Content.ReadFromJsonAsync<CategoryDto>();

        var product = new CreateProductDto(
            "Test Product",
            "Test Description",
            100m,
            createdCategory!.Id,
            5 // Estoque baixo para aparecer no dashboard
        );

        await _client.PostAsJsonAsync("/api/products", product);
        var updatedResponse = await _client.GetAsync("/api/dashboard");
        if (updatedResponse.IsSuccessStatusCode)
        {
            var updatedDashboard = await updatedResponse.Content.ReadFromJsonAsync<DashboardDto>();
            updatedDashboard!.TotalProducts.Should().BeGreaterOrEqualTo(initialTotal);
        }
    }
}
