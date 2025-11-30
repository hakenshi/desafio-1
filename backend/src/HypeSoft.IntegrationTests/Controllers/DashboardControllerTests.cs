using System.Net;
using FluentAssertions;

namespace HypeSoft.IntegrationTests.Controllers;

public class DashboardControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DashboardControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDashboard_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/dashboard");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
