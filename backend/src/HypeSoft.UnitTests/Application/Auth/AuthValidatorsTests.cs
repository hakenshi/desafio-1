using FluentAssertions;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Auth.Commands;
using HypeSoft.Application.Auth.Validators;

namespace HypeSoft.UnitTests.Application.Auth;

public class AuthDtoTests
{
    #region LoginRequestDto Tests

    [Fact]
    public void LoginRequestDto_DefaultValues_ShouldHaveEmptyStrings()
    {
        // Act
        var request = new LoginRequestDto();

        // Assert
        request.Email.Should().BeEmpty();
        request.Password.Should().BeEmpty();
    }

    [Fact]
    public void LoginRequestDto_WithValues_ShouldSetCorrectly()
    {
        // Act
        var request = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "password123"
        };

        // Assert
        request.Email.Should().Be("test@example.com");
        request.Password.Should().Be("password123");
    }

    #endregion

    #region RegisterRequestDto Tests

    [Fact]
    public void RegisterRequestDto_DefaultValues_ShouldHaveEmptyStrings()
    {
        // Act
        var request = new RegisterRequestDto();

        // Assert
        request.Username.Should().BeEmpty();
        request.Email.Should().BeEmpty();
        request.Password.Should().BeEmpty();
        request.FirstName.Should().BeNull();
        request.LastName.Should().BeNull();
    }

    [Fact]
    public void RegisterRequestDto_WithValues_ShouldSetCorrectly()
    {
        // Act
        var request = new RegisterRequestDto
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

    #region RefreshTokenRequestDto Tests

    [Fact]
    public void RefreshTokenRequestDto_DefaultValues_ShouldHaveEmptyString()
    {
        // Act
        var request = new RefreshTokenRequestDto();

        // Assert
        request.RefreshToken.Should().BeEmpty();
    }

    [Fact]
    public void RefreshTokenRequestDto_WithValue_ShouldSetCorrectly()
    {
        // Act
        var request = new RefreshTokenRequestDto
        {
            RefreshToken = "refresh-token-value"
        };

        // Assert
        request.RefreshToken.Should().Be("refresh-token-value");
    }

    #endregion

    #region TokenResponseDto Tests

    [Fact]
    public void TokenResponseDto_DefaultValues_ShouldHaveCorrectDefaults()
    {
        // Act
        var response = new TokenResponseDto();

        // Assert
        response.AccessToken.Should().BeEmpty();
        response.RefreshToken.Should().BeEmpty();
        response.ExpiresIn.Should().Be(0);
        response.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public void TokenResponseDto_WithValues_ShouldSetCorrectly()
    {
        // Act
        var response = new TokenResponseDto
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

    #region UserInfoDto Tests

    [Fact]
    public void UserInfoDto_DefaultValues_ShouldHaveCorrectDefaults()
    {
        // Act
        var userInfo = new UserInfoDto();

        // Assert
        userInfo.Id.Should().BeEmpty();
        userInfo.Username.Should().BeEmpty();
        userInfo.Email.Should().BeEmpty();
        userInfo.FirstName.Should().BeNull();
        userInfo.LastName.Should().BeNull();
        userInfo.Roles.Should().BeEmpty();
    }

    [Fact]
    public void UserInfoDto_WithValues_ShouldSetCorrectly()
    {
        // Act
        var userInfo = new UserInfoDto
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

public class AuthValidatorsTests
{
    #region LoginCommandValidator Tests

    [Fact]
    public void LoginCommandValidator_ValidCommand_ShouldPass()
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("test@example.com", "password123");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void LoginCommandValidator_EmptyEmail_ShouldFail()
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("", "password123");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void LoginCommandValidator_InvalidEmail_ShouldFail()
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("invalid-email", "password123");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void LoginCommandValidator_ShortPassword_ShouldFail()
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("test@example.com", "12345");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    #endregion

    #region RegisterCommandValidator Tests

    [Fact]
    public void RegisterCommandValidator_ValidCommand_ShouldPass()
    {
        // Arrange
        var validator = new RegisterCommandValidator();
        var request = new RegisterRequestDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };
        var command = new RegisterCommand(request);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RegisterCommandValidator_ShortUsername_ShouldFail()
    {
        // Arrange
        var validator = new RegisterCommandValidator();
        var request = new RegisterRequestDto
        {
            Username = "ab",
            Email = "test@example.com",
            Password = "password123"
        };
        var command = new RegisterCommand(request);

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    #endregion

    #region RefreshTokenCommandValidator Tests

    [Fact]
    public void RefreshTokenCommandValidator_ValidCommand_ShouldPass()
    {
        // Arrange
        var validator = new RefreshTokenCommandValidator();
        var command = new RefreshTokenCommand("valid-refresh-token");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RefreshTokenCommandValidator_EmptyToken_ShouldFail()
    {
        // Arrange
        var validator = new RefreshTokenCommandValidator();
        var command = new RefreshTokenCommand("");

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    #endregion
}
