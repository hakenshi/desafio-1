using FluentAssertions;
using HypeSoft.API.Controllers;

namespace HypeSoft.UnitTests.Application.Auth;

public class AuthValidatorsTests
{
    #region LoginRequest Tests

    [Fact]
    public void LoginRequest_ValidData_ShouldCreateInstance()
    {
        // Act
        var request = new LoginRequest("username", "password");

        // Assert
        request.Username.Should().Be("username");
        request.Password.Should().Be("password");
    }

    [Fact]
    public void LoginRequest_EmptyUsername_ShouldStillCreateInstance()
    {
        // Act
        var request = new LoginRequest("", "password");

        // Assert
        request.Username.Should().BeEmpty();
        request.Password.Should().Be("password");
    }

    [Fact]
    public void LoginRequest_EmptyPassword_ShouldStillCreateInstance()
    {
        // Act
        var request = new LoginRequest("username", "");

        // Assert
        request.Username.Should().Be("username");
        request.Password.Should().BeEmpty();
    }

    #endregion

    #region RegisterRequest Tests

    [Fact]
    public void RegisterRequest_FullData_ShouldCreateInstance()
    {
        // Act
        var request = new RegisterRequest(
            "username",
            "email@test.com",
            "password123",
            "First",
            "Last");

        // Assert
        request.Username.Should().Be("username");
        request.Email.Should().Be("email@test.com");
        request.Password.Should().Be("password123");
        request.FirstName.Should().Be("First");
        request.LastName.Should().Be("Last");
    }

    [Fact]
    public void RegisterRequest_MinimalData_ShouldCreateInstance()
    {
        // Act
        var request = new RegisterRequest(
            "username",
            "email@test.com",
            "password123");

        // Assert
        request.Username.Should().Be("username");
        request.Email.Should().Be("email@test.com");
        request.Password.Should().Be("password123");
        request.FirstName.Should().BeNull();
        request.LastName.Should().BeNull();
    }

    #endregion

    #region RefreshTokenRequest Tests

    [Fact]
    public void RefreshTokenRequest_ValidToken_ShouldCreateInstance()
    {
        // Act
        var request = new RefreshTokenRequest("refresh-token-value");

        // Assert
        request.RefreshToken.Should().Be("refresh-token-value");
    }

    [Fact]
    public void RefreshTokenRequest_EmptyToken_ShouldStillCreateInstance()
    {
        // Act
        var request = new RefreshTokenRequest("");

        // Assert
        request.RefreshToken.Should().BeEmpty();
    }

    #endregion

    #region TokenResponse Tests

    [Fact]
    public void TokenResponse_DefaultValues_ShouldHaveEmptyStrings()
    {
        // Act
        var response = new TokenResponse();

        // Assert
        response.AccessToken.Should().BeEmpty();
        response.RefreshToken.Should().BeEmpty();
        response.ExpiresIn.Should().Be(0);
        response.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public void TokenResponse_WithValues_ShouldSetCorrectly()
    {
        // Act
        var response = new TokenResponse
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            ExpiresIn = 3600,
            TokenType = "Bearer"
        };

        // Assert
        response.AccessToken.Should().Be("access-token");
        response.RefreshToken.Should().Be("refresh-token");
        response.ExpiresIn.Should().Be(3600);
        response.TokenType.Should().Be("Bearer");
    }

    #endregion

    #region UserInfo Tests

    [Fact]
    public void UserInfo_DefaultValues_ShouldHaveEmptyStrings()
    {
        // Act
        var userInfo = new UserInfo();

        // Assert
        userInfo.Id.Should().BeEmpty();
        userInfo.Username.Should().BeEmpty();
        userInfo.Email.Should().BeEmpty();
        userInfo.FirstName.Should().BeNull();
        userInfo.LastName.Should().BeNull();
        userInfo.Roles.Should().BeEmpty();
    }

    [Fact]
    public void UserInfo_WithValues_ShouldSetCorrectly()
    {
        // Act
        var userInfo = new UserInfo
        {
            Id = "user-123",
            Username = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Roles = new List<string> { "user", "admin" }
        };

        // Assert
        userInfo.Id.Should().Be("user-123");
        userInfo.Username.Should().Be("testuser");
        userInfo.Email.Should().Be("test@example.com");
        userInfo.FirstName.Should().Be("Test");
        userInfo.LastName.Should().Be("User");
        userInfo.Roles.Should().HaveCount(2);
        userInfo.Roles.Should().Contain("user");
        userInfo.Roles.Should().Contain("admin");
    }

    #endregion
}
