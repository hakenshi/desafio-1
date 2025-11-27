using FluentAssertions;
using HypeSoft.Application.Products.Commands;
using HypeSoft.Domain.Repositories;
using Moq;

namespace HypeSoft.UnitTests.Application.Products;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _handler = new DeleteProductCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidId_ShouldCallDeleteAsync()
    {
        // Arrange
        var command = new DeleteProductCommand("product-1");

        _repositoryMock
            .Setup(x => x.DeleteAsync("product-1", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            x => x.DeleteAsync("product-1", It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnUnitValue()
    {
        // Arrange
        var command = new DeleteProductCommand("product-1");

        _repositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
    }
}
