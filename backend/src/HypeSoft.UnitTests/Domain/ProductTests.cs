using FluentAssertions;
using HypeSoft.Domain.Entities;

namespace HypeSoft.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void IsLowStock_WhenStockIsLessThan10_ShouldReturnTrue()
    {
        // Arrange
        var product = new Product
        {
            Id = "1",
            Name = "Test Product",
            Description = "Test Description",
            Price = 100,
            CategoryId = "cat1",
            StockQuantity = 5,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = product.IsLowStock();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsLowStock_WhenStockIs10OrMore_ShouldReturnFalse()
    {
        // Arrange
        var product = new Product
        {
            Id = "1",
            Name = "Test Product",
            Description = "Test Description",
            Price = 100,
            CategoryId = "cat1",
            StockQuantity = 10,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var result = product.IsLowStock();

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(9)]
    public void IsLowStock_WithVariousLowStockValues_ShouldReturnTrue(int stockQuantity)
    {
        // Arrange
        var product = new Product { StockQuantity = stockQuantity };

        // Act & Assert
        product.IsLowStock().Should().BeTrue();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void IsLowStock_WithVariousHighStockValues_ShouldReturnFalse(int stockQuantity)
    {
        // Arrange
        var product = new Product { StockQuantity = stockQuantity };

        // Act & Assert
        product.IsLowStock().Should().BeFalse();
    }
}
