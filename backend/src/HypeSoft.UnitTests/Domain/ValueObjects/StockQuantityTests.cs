using FluentAssertions;
using HypeSoft.Domain.ValueObjects;

namespace HypeSoft.UnitTests.Domain.ValueObjects;

public class StockQuantityTests
{
    [Fact]
    public void Create_WithValidQuantity_ShouldCreateStock()
    {
        // Act
        var stock = StockQuantity.Create(50);

        // Assert
        stock.Quantity.Should().Be(50);
    }

    [Fact]
    public void Create_WithNegativeQuantity_ShouldThrowException()
    {
        // Act & Assert
        var act = () => StockQuantity.Create(-5);
        act.Should().Throw<ArgumentException>().WithParameterName("quantity");
    }

    [Fact]
    public void IsLowStock_WhenBelowThreshold_ShouldReturnTrue()
    {
        // Arrange
        var stock = StockQuantity.Create(5);

        // Assert
        stock.IsLowStock.Should().BeTrue();
    }

    [Fact]
    public void IsLowStock_WhenAboveThreshold_ShouldReturnFalse()
    {
        // Arrange
        var stock = StockQuantity.Create(15);

        // Assert
        stock.IsLowStock.Should().BeFalse();
    }

    [Fact]
    public void IsOutOfStock_WhenZero_ShouldReturnTrue()
    {
        // Arrange
        var stock = StockQuantity.Zero;

        // Assert
        stock.IsOutOfStock.Should().BeTrue();
    }

    [Fact]
    public void Add_WithValidQuantity_ShouldIncreaseStock()
    {
        // Arrange
        var stock = StockQuantity.Create(10);

        // Act
        var result = stock.Add(5);

        // Assert
        result.Quantity.Should().Be(15);
    }

    [Fact]
    public void Add_WithNegativeQuantity_ShouldThrowException()
    {
        // Arrange
        var stock = StockQuantity.Create(10);

        // Act & Assert
        var act = () => stock.Add(-5);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Subtract_WithValidQuantity_ShouldDecreaseStock()
    {
        // Arrange
        var stock = StockQuantity.Create(10);

        // Act
        var result = stock.Subtract(3);

        // Assert
        result.Quantity.Should().Be(7);
    }

    [Fact]
    public void Subtract_WithInsufficientStock_ShouldThrowException()
    {
        // Arrange
        var stock = StockQuantity.Create(5);

        // Act & Assert
        var act = () => stock.Subtract(10);
        act.Should().Throw<InvalidOperationException>();
    }
}
