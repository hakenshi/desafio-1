using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Mappings;
using HypeSoft.Application.Products.Commands;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;

namespace HypeSoft.UnitTests.Application.Products;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _handler = new UpdateProductCommandHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ExistingProduct_ShouldUpdateAndReturnProductDto()
    {
        // Arrange
        var existingProduct = Product.Create("Old Name", "Old Description", 50m, "cat-1", 10);

        var updateDto = new UpdateProductDto(
            "New Name",
            "New Description",
            100m,
            "cat-2",
            20
        );

        var command = new UpdateProductCommand(existingProduct.Id, updateDto);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");
        result.Description.Should().Be("New Description");
        result.Price.Should().Be(100m);
        result.CategoryId.Should().Be("cat-2");
        result.StockQuantity.Should().Be(20);

        _repositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_NonExistingProduct_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateProductDto("Name", "Desc", 10m, "cat1", 5);
        var command = new UpdateProductCommand("non-existing-id", updateDto);

        _repositoryMock
            .Setup(x => x.GetByIdAsync("non-existing-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*non-existing-id*");
    }

    [Fact]
    public async Task Handle_ShouldUpdateUpdatedAtTimestamp()
    {
        // Arrange
        var existingProduct = Product.Create("Name", "Desc", 10m, "cat1", 5);

        var updateDto = new UpdateProductDto("New Name", "New Desc", 20m, "cat2", 10);
        var command = new UpdateProductCommand(existingProduct.Id, updateDto);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_InvalidUpdateData_ShouldThrowArgumentException()
    {
        // Arrange
        var existingProduct = Product.Create("Name", "Desc", 10m, "cat1", 5);

        var updateDto = new UpdateProductDto("", "New Desc", 20m, "cat2", 10); // Empty name
        var command = new UpdateProductCommand(existingProduct.Id, updateDto);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*name*");
    }

    [Fact]
    public async Task Handle_NegativePrice_ShouldThrowArgumentException()
    {
        // Arrange
        var existingProduct = Product.Create("Name", "Desc", 10m, "cat1", 5);

        var updateDto = new UpdateProductDto("New Name", "New Desc", -20m, "cat2", 10);
        var command = new UpdateProductCommand(existingProduct.Id, updateDto);

        _repositoryMock
            .Setup(x => x.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*price*");
    }
}
