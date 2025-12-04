using FluentAssertions;
using HypeSoft.Application.Dashboard.Queries;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;
using Xunit;

namespace HypeSoft.UnitTests.Application.Dashboard;

public class GetRecentAuditLogsQueryHandlerTests
{
    private readonly Mock<IAuditLogRepository> _auditLogRepositoryMock;
    private readonly GetRecentAuditLogsQueryHandler _handler;

    public GetRecentAuditLogsQueryHandlerTests()
    {
        _auditLogRepositoryMock = new Mock<IAuditLogRepository>();
        _handler = new GetRecentAuditLogsQueryHandler(_auditLogRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsRecentAuditLogs()
    {
        var logs = new List<AuditLog>
        {
            AuditLog.Create("user-1", "admin", "Create", "Product", "prod-1", "Test Product", "Created product"),
            AuditLog.Create("user-2", "manager", "Update", "Category", "cat-1", "Test Category", "Updated category"),
        };

        _auditLogRepositoryMock
            .Setup(x => x.GetRecentAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        var query = new GetRecentAuditLogsQuery(10);
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Action.Should().Be("Create");
    }

    [Fact]
    public async Task Handle_EmptyLogs_ReturnsEmptyList()
    {
        _auditLogRepositoryMock
            .Setup(x => x.GetRecentAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AuditLog>());

        var query = new GetRecentAuditLogsQuery(10);
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_CustomCount_UsesCorrectCount()
    {
        _auditLogRepositoryMock
            .Setup(x => x.GetRecentAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AuditLog>());

        var query = new GetRecentAuditLogsQuery(5);
        await _handler.Handle(query, CancellationToken.None);
        _auditLogRepositoryMock.Verify(x => x.GetRecentAsync(5, It.IsAny<CancellationToken>()), Times.Once);
    }
}
