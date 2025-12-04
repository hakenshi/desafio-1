using FluentAssertions;
using HypeSoft.Application.Auth.Queries;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Interfaces;
using Moq;

namespace HypeSoft.UnitTests.Application.Auth;

public class GetUsersQueryHandlerTests
{
    private readonly Mock<IKeycloakService> _keycloakServiceMock;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _keycloakServiceMock = new Mock<IKeycloakService>();
        _handler = new GetUsersQueryHandler(_keycloakServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUsersFromKeycloakService()
    {
        var expectedUsers = new List<KeycloakUserDto>
        {
            new() { Id = "1", Username = "admin", Email = "admin@test.com", Role = "admin", Enabled = true },
            new() { Id = "2", Username = "manager", Email = "manager@test.com", Role = "manager", Enabled = true },
            new() { Id = "3", Username = "user", Email = "user@test.com", Role = "user", Enabled = true }
        };

        _keycloakServiceMock
            .Setup(x => x.GetUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUsers);

        var query = new GetUsersQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(3);
        result.Should().Contain(u => u.Role == "admin");
        result.Should().Contain(u => u.Role == "manager");
        result.Should().Contain(u => u.Role == "user");
    }

    [Fact]
    public async Task Handle_WhenNoUsers_ShouldReturnEmptyList()
    {
        _keycloakServiceMock
            .Setup(x => x.GetUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<KeycloakUserDto>());

        var query = new GetUsersQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallKeycloakServiceOnce()
    {
        _keycloakServiceMock
            .Setup(x => x.GetUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<KeycloakUserDto>());

        var query = new GetUsersQuery();

        await _handler.Handle(query, CancellationToken.None);

        _keycloakServiceMock.Verify(x => x.GetUsersAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnUsersWithCorrectRoles()
    {
        var users = new List<KeycloakUserDto>
        {
            new() { Id = "1", Username = "admin", Email = "admin@test.com", Role = "admin" },
            new() { Id = "2", Username = "manager", Email = "manager@test.com", Role = "manager" },
            new() { Id = "3", Username = "user1", Email = "user1@test.com", Role = "user" },
            new() { Id = "4", Username = "user2", Email = "user2@test.com", Role = "user" }
        };

        _keycloakServiceMock
            .Setup(x => x.GetUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        var query = new GetUsersQuery();

        var result = await _handler.Handle(query, CancellationToken.None);

        var resultList = result.ToList();
        resultList.Count(u => u.Role == "admin").Should().Be(1);
        resultList.Count(u => u.Role == "manager").Should().Be(1);
        resultList.Count(u => u.Role == "user").Should().Be(2);
    }
}
