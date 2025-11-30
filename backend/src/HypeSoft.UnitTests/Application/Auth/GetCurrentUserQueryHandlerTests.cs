using FluentAssertions;
using HypeSoft.Application.Auth.Queries;
using Microsoft.Extensions.Logging;
using Moq;

namespace HypeSoft.UnitTests.Application.Auth;

public class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<ILogger<GetCurrentUserQueryHandler>> _loggerMock;
    private readonly GetCurrentUserQueryHandler _handler;

    // Sample JWT token with all claims (decoded payload below)
    // {
    //   "sub": "34e654e6-3877-4489-a79f-a545fbb3053c",
    //   "preferred_username": "admin",
    //   "email": "admin@hypesoft.com",
    //   "given_name": "Admin",
    //   "family_name": "User",
    //   "realm_access": {"roles": ["admin", "user"]}
    // }
    private const string ValidToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzNGU2NTRlNi0zODc3LTQ0ODktYTc5Zi1hNTQ1ZmJiMzA1M2MiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJhZG1pbiIsImVtYWlsIjoiYWRtaW5AaHlwZXNvZnQuY29tIiwiZ2l2ZW5fbmFtZSI6IkFkbWluIiwiZmFtaWx5X25hbWUiOiJVc2VyIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImFkbWluIiwidXNlciJdfX0.signature";

    public GetCurrentUserQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetCurrentUserQueryHandler>>();
        _handler = new GetCurrentUserQueryHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidToken_ShouldReturnUserInfoWithAllFields()
    {
        // Arrange
        var query = new GetCurrentUserQuery(ValidToken);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Id.Should().Be("34e654e6-3877-4489-a79f-a545fbb3053c");
        result.Username.Should().Be("admin");
        result.Email.Should().Be("admin@hypesoft.com");
        result.FirstName.Should().Be("Admin");
        result.LastName.Should().Be("User");
        result.Roles.Should().Contain("admin");
        result.Roles.Should().Contain("user");
    }

    [Fact]
    public async Task Handle_WithValidToken_ShouldNotHaveEmptyRequiredFields()
    {
        // Arrange
        var query = new GetCurrentUserQuery(ValidToken);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert - These assertions MUST pass, otherwise the API is returning invalid data
        result.Id.Should().NotBeNullOrEmpty("API must return a valid user ID");
        result.Username.Should().NotBeNullOrEmpty("API must return a valid username");
        result.Email.Should().NotBeNullOrEmpty("API must return a valid email");
        result.Roles.Should().NotBeEmpty("API must return user roles");
    }

    [Theory]
    [InlineData("", "", "", null, null)] // All empty
    [InlineData("id-123", "", "", null, null)] // Only ID
    [InlineData("", "admin", "", null, null)] // Only username
    public void UserInfoDto_WithEmptyRequiredFields_ShouldBeInvalid(
        string id, string username, string email, string? firstName, string? lastName)
    {
        // This test documents that UserInfoDto with empty required fields is invalid
        var userInfo = new HypeSoft.Application.DTOs.UserInfoDto
        {
            Id = id,
            Username = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Roles = new List<string>()
        };

        // At minimum, Id, Username and Email should be populated
        var isValid = !string.IsNullOrEmpty(userInfo.Id) 
                   && !string.IsNullOrEmpty(userInfo.Username) 
                   && !string.IsNullOrEmpty(userInfo.Email);

        isValid.Should().BeFalse("UserInfoDto with empty required fields should be considered invalid");
    }

    [Fact]
    public async Task Handle_WithEmptyToken_ShouldReturnEmptyUserInfo()
    {
        // Arrange
        var query = new GetCurrentUserQuery("");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Id.Should().BeEmpty();
        result.Username.Should().BeEmpty();
        result.Email.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithInvalidToken_ShouldReturnEmptyUserInfo()
    {
        // Arrange
        var query = new GetCurrentUserQuery("invalid-token");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Id.Should().BeEmpty();
    }
}
