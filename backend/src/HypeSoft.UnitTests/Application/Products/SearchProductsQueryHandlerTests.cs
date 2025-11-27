using AutoMapper;
using FluentAssertions;
using HypeSoft.Application.Mappings;
using HypeSoft.Application.Products.Queries;
using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using Moq;

namespace HypeSoft.UnitTests.Application.Products;

public class SearchProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly SearchProductsQueryHandler _handler;

    public SearchProductsQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _handler = new SearchProductsQueryHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task Handle_ValidSearchTerm_ShouldReturnMatchingProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = "1", Name = "Notebook Dell", Description = "Desc", Price = 100m, CategoryId = "cat1", StockQuantity = 10, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = "2", Name = "Notebook HP", Description = "Desc", Price = 200m, CategoryId = "cat1", StockQuantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        var query = new SearchProductsQuery("Notebook");

        _repositoryMock
            .Setup(x => x.SearchByNameAsync("Notebook", It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Name.Contains("Notebook"));
    }

    [Fact]
    public async Task Handle_NoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new SearchProductsQuery("NonExistent");

        _repositoryMock
            .Setup(x => x.SearchByNameAsync("NonExistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
