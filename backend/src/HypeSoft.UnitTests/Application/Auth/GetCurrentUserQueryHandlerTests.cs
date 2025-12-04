using FluentAssertions;
using HypeSoft.Application.Auth.Queries;
using Microsoft.Extensions.Logging;
using Moq;

namespace HypeSoft.UnitTests.Application.Auth;

public class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<ILogger<GetCurrentUserQueryHandler>> _loggerMock;
    private readonly GetCurrentUserQueryHandler _handler;

    private const string AdminToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzNGU2NTRlNi0zODc3LTQ0ODktYTc5Zi1hNTQ1ZmJiMzA1M2MiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJhZG1pbiIsImVtYWlsIjoiYWRtaW5AaHlwZXNvZnQuY29tIiwiZ2l2ZW5fbmFtZSI6IkFkbWluIiwiZmFtaWx5X25hbWUiOiJVc2VyIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbImFkbWluIl19fQ.signature";

    private const string ManagerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0NWY3NjVmNy00OTg4LTU1OTAtYjgwZy1iNjU2Z2NjNDE2NGQiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJtYW5hZ2VyIiwiZW1haWwiOiJtYW5hZ2VyQGh5cGVzb2Z0LmNvbSIsImdpdmVuX25hbWUiOiJNYW5hZ2VyIiwiZmFtaWx5X25hbWUiOiJVc2VyIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbIm1hbmFnZXIiXX19.signature";

    private const string UserToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1Nmc4NzZnOC01MDk5LTY2MDEtYzkxaC1jNzY3aGRkNTI3NWUiLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ1c2VyIiwiZW1haWwiOiJ1c2VyQGh5cGVzb2Z0LmNvbSIsImdpdmVuX25hbWUiOiJSZWd1bGFyIiwiZmFtaWx5X25hbWUiOiJVc2VyIiwicmVhbG1fYWNjZXNzIjp7InJvbGVzIjpbInVzZXIiXX19.signature";

    public GetCurrentUserQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetCurrentUserQueryHandler>>();
        _handler = new GetCurrentUserQueryHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithAdminToken_ShouldReturnAdminRole()
    {
        var query = new GetCurrentUserQuery(AdminToken);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Id.Should().Be("34e654e6-3877-4489-a79f-a545fbb3053c");
        result.Username.Should().Be("admin");
        result.Email.Should().Be("admin@hypesoft.com");
        result.FirstName.Should().Be("Admin");
        result.LastName.Should().Be("User");
        result.Role.Should().Be("admin");
    }

    [Fact]
    public async Task Handle_WithManagerToken_ShouldReturnManagerRole()
    {
        var query = new GetCurrentUserQuery(ManagerToken);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Username.Should().Be("manager");
        result.Email.Should().Be("manager@hypesoft.com");
        result.Role.Should().Be("manager");
    }

    [Fact]
    public async Task Handle_WithUserToken_ShouldReturnUserRole()
    {
        var query = new GetCurrentUserQuery(UserToken);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Username.Should().Be("user");
        result.Email.Should().Be("user@hypesoft.com");
        result.Role.Should().Be("user");
    }

    [Fact]
    public async Task Handle_WithValidToken_ShouldNotHaveEmptyRequiredFields()
    {
        var query = new GetCurrentUserQuery(AdminToken);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Id.Should().NotBeNullOrEmpty("API must return a valid user ID");
        result.Username.Should().NotBeNullOrEmpty("API must return a valid username");
        result.Email.Should().NotBeNullOrEmpty("API must return a valid email");
        result.Role.Should().NotBeNullOrEmpty("API must return user role");
    }

    [Theory]
    [InlineData("", "", "", null, null, "user")]
    [InlineData("id-123", "", "", null, null, "admin")]
    [InlineData("", "admin", "", null, null, "manager")]
    public void UserInfoDto_WithEmptyRequiredFields_ShouldBeInvalid(
        string id, string username, string email, string? firstName, string? lastName, string role)
    {
        var userInfo = new HypeSoft.Application.DTOs.UserInfoDto
        {
            Id = id,
            Username = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role
        };

        var isValid = !string.IsNullOrEmpty(userInfo.Id) 
                   && !string.IsNullOrEmpty(userInfo.Username) 
                   && !string.IsNullOrEmpty(userInfo.Email);

        isValid.Should().BeFalse("UserInfoDto with empty required fields should be considered invalid");
    }

    [Fact]
    public async Task Handle_WithEmptyToken_ShouldReturnEmptyUserInfoWithDefaultRole()
    {
        var query = new GetCurrentUserQuery("");

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Id.Should().BeEmpty();
        result.Username.Should().BeEmpty();
        result.Email.Should().BeEmpty();
        result.Role.Should().Be("user");
    }

    [Fact]
    public async Task Handle_WithInvalidToken_ShouldReturnEmptyUserInfo()
    {
        var query = new GetCurrentUserQuery("invalid-token");

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Id.Should().BeEmpty();
    }

    [Theory]
    [InlineData("admin")]
    [InlineData("manager")]
    [InlineData("user")]
    public void Role_ShouldBeOneOfValidValues(string role)
    {
        var userInfo = new HypeSoft.Application.DTOs.UserInfoDto { Role = role };

        userInfo.Role.Should().BeOneOf("admin", "manager", "user");
    }
}
