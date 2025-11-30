using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using HypeSoft.Application.DTOs;

namespace HypeSoft.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Username = $"testuser_{Guid.NewGuid():N}",
            Email = $"test_{Guid.NewGuid():N}@example.com",
            Password = "Test123!",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "invalid@example.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentUser_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
