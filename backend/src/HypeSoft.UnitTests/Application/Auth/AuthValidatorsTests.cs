using FluentAssertions;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Auth.Commands;
using HypeSoft.Application.Auth.Validators;

namespace HypeSoft.UnitTests.Application.Auth;

public class AuthValidatorsTests
{
    #region LoginCommandValidator Tests

    [Fact]
    public void LoginCommandValidator_ValidCommand_ShouldPass()
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("test@example.com", "password123");

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void LoginCommandValidator_EmptyEmail_ShouldFail()
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("", "password123");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void LoginCommandValidator_InvalidEmail_ShouldFail()
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("invalid-email", "password123");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void LoginCommandValidator_ShortPassword_ShouldFail()
    {
        var validator = new LoginCommandValidator();
        var command = new LoginCommand("test@example.com", "12345");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    #endregion


    #region RegisterCommandValidator Tests

    [Fact]
    public void RegisterCommandValidator_ValidCommand_ShouldPass()
    {
        var validator = new RegisterCommandValidator();
        var request = new RegisterRequestDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };
        var command = new RegisterCommand(request);

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RegisterCommandValidator_ShortUsername_ShouldFail()
    {
        var validator = new RegisterCommandValidator();
        var request = new RegisterRequestDto
        {
            Username = "ab",
            Email = "test@example.com",
            Password = "password123"
        };
        var command = new RegisterCommand(request);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    #endregion

    #region RefreshTokenCommandValidator Tests

    [Fact]
    public void RefreshTokenCommandValidator_ValidCommand_ShouldPass()
    {
        var validator = new RefreshTokenCommandValidator();
        var command = new RefreshTokenCommand("valid-refresh-token");

        var result = validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RefreshTokenCommandValidator_EmptyToken_ShouldFail()
    {
        var validator = new RefreshTokenCommandValidator();
        var command = new RefreshTokenCommand("");

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
    }

    #endregion
}
