using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.DTOs;
using HypeSoft.Application.Mappings;
using HypeSoft.Application.Products.Commands;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;

namespace HypeSoft.UnitTests.Application.Products;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _handler = new CreateProductCommandHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidProduct_ShouldCreateAndReturnProductDto()
    {
        // Arrange
        var createDto = new CreateProductDto(
            "Test Product",
            "Test Description",
            99.99m,
            "category-1",
            50
        );

        var command = new CreateProductCommand(createDto);

        _repositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken ct) => p);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Product");
        result.Description.Should().Be("Test Description");
        result.Price.Should().Be(99.99m);
        result.CategoryId.Should().Be("category-1");
        result.StockQuantity.Should().Be(50);
        result.Id.Should().NotBeNullOrEmpty();

        _repositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ProductWithLowStock_ShouldSetIsLowStockTrue()
    {
        // Arrange
        var createDto = new CreateProductDto(
            "Low Stock Product",
            "Description",
            50m,
            "category-1",
            5
        );

        var command = new CreateProductCommand(createDto);

        _repositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken ct) => p);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsLowStock.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldSetCreatedAndUpdatedDates()
    {
        // Arrange
        var createDto = new CreateProductDto("Product", "Desc", 10m, "cat1", 20);
        var command = new CreateProductCommand(createDto);

        _repositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product p, CancellationToken ct) => p);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
