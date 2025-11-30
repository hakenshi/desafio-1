using FluentAssertions;
using HypeSoft.API.Controllers;

namespace HypeSoft.UnitTests.Application.Auth;

public class AuthDtoTests
{
    #region LoginRequest Tests

    [Fact]
    public void LoginRequest_DefaultValues_ShouldHaveEmptyStrings()
    {
        // Act
        var request = new LoginRequest();

        // Assert
        request.Email.Should().BeEmpty();
        request.Password.Should().BeEmpty();
    }

    [Fact]
    public void LoginRequest_WithValues_ShouldSetCorrectly()
    {
        // Act
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123"
        };

        // Assert
        request.Email.Should().Be("test@example.com");
        request.Password.Should().Be("password123");
    }

    #endregion

    #region RegisterRequest Tests

    [Fact]
    public void RegisterRequest_DefaultValues_ShouldHaveEmptyStrings()
    {
        // Act
        var request = new RegisterRequest();

        // Assert
        request.Username.Should().BeEmpty();
        request.Email.Should().BeEmpty();
        request.Password.Should().BeEmpty();
        request.FirstName.Should().BeNull();
        request.LastName.Should().BeNull();
    }

    [Fact]
    public void RegisterRequest_WithValues_ShouldSetCorrectly()
    {
        // Act
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            FirstName = "Test",
            LastName = "User"
        };

        // Assert
        request.Username.Should().Be("testuser");
        request.Email.Should().Be("test@example.com");
        request.Password.Should().Be("password123");
        request.FirstName.Should().Be("Test");
        request.LastName.Should().Be("User");
    }

    #endregion

    #region RefreshTokenRequest Tests

    [Fact]
    public void RefreshTokenRequest_DefaultValues_ShouldHaveEmptyString()
    {
        // Act
        var request = new RefreshTokenRequest();

        // Assert
        request.RefreshToken.Should().BeEmpty();
    }

    [Fact]
    public void RefreshTokenRequest_WithValue_ShouldSetCorrectly()
    {
        // Act
        var request = new RefreshTokenRequest
        {
            RefreshToken = "refresh-token-value"
        };

        // Assert
        request.RefreshToken.Should().Be("refresh-token-value");
    }

    #endregion

    #region TokenResponse Tests

    [Fact]
    public void TokenResponse_DefaultValues_ShouldHaveCorrectDefaults()
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
    public void UserInfo_DefaultValues_ShouldHaveCorrectDefaults()
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
