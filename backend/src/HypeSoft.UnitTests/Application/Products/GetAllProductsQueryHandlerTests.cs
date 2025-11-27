using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.Mappings;
using HypeSoft.Application.Products.Queries;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;

namespace HypeSoft.UnitTests.Application.Products;

public class GetAllProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GetAllProductsQueryHandler _handler;

    public GetAllProductsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _handler = new GetAllProductsQueryHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = "1", Name = "Product 1", Description = "Desc 1", Price = 10m, CategoryId = "cat1", StockQuantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = "2", Name = "Product 2", Description = "Desc 2", Price = 20m, CategoryId = "cat2", StockQuantity = 15, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        var query = new GetAllProductsQuery(1, 10);

        _repositoryMock
            .Setup(x => x.GetAllAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Product 1");
        result.Should().Contain(p => p.Name == "Product 2");
    }

    [Fact]
    public async Task Handle_WithPagination_ShouldPassCorrectParameters()
    {
        // Arrange
        var query = new GetAllProductsQuery(2, 20);

        _repositoryMock
            .Setup(x => x.GetAllAsync(2, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(
            x => x.GetAllAsync(2, 20, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
