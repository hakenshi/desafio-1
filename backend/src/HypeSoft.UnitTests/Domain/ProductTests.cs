using FluentAssertions;
using HypeSoft.Domain.Entities;

namespace HypeSoft.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_ValidData_ShouldCreateProduct()
    {
        // Act
        var product = Product.Create("Test Product", "Test Description", 100m, "cat1", 5);

        // Assert
        product.Should().NotBeNull();
        product.Id.Should().NotBeNullOrEmpty();
        product.Name.Should().Be("Test Product");
        product.Description.Should().Be("Test Description");
        product.Price.Should().Be(100m);
        product.CategoryId.Should().Be("cat1");
        product.StockQuantity.Should().Be(5);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_EmptyName_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Product.Create("", "Description", 100m, "cat1", 5);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*name*");
    }

    [Fact]
    public void Create_EmptyDescription_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Product.Create("Name", "", 100m, "cat1", 5);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*description*");
    }

    [Fact]
    public void Create_NegativePrice_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Product.Create("Name", "Description", -10m, "cat1", 5);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*price*");
    }

    [Fact]
    public void Create_NegativeStock_ShouldThrowArgumentException()
    {
        // Act
        var act = () => Product.Create("Name", "Description", 100m, "cat1", -5);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Stock*");
    }

    [Fact]
    public void Update_ValidData_ShouldUpdateProduct()
    {
        // Arrange
        var product = Product.Create("Original", "Original Desc", 50m, "cat1", 10);
        var originalCreatedAt = product.CreatedAt;

        // Act
        product.Update("Updated", "Updated Desc", 100m, "cat2", 20);

        // Assert
        product.Name.Should().Be("Updated");
        product.Description.Should().Be("Updated Desc");
        product.Price.Should().Be(100m);
        product.CategoryId.Should().Be("cat2");
        product.StockQuantity.Should().Be(20);
        product.CreatedAt.Should().Be(originalCreatedAt);
        product.UpdatedAt.Should().BeAfter(originalCreatedAt);
    }

    [Fact]
    public void Update_EmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var product = Product.Create("Name", "Description", 100m, "cat1", 5);

        // Act
        var act = () => product.Update("", "Description", 100m, "cat1", 5);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*name*");
    }

    [Fact]
    public void IsLowStock_WhenStockIsLessThanThreshold_ShouldReturnTrue()
    {
        // Arrange
        var product = Product.Create("Test Product", "Test Description", 100m, "cat1", 5);

        // Act
        var result = product.IsLowStock();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsLowStock_WhenStockIsEqualOrAboveThreshold_ShouldReturnFalse()
    {
        // Arrange
        var product = Product.Create("Test Product", "Test Description", 100m, "cat1", 10);

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
        var product = Product.Create("Test", "Desc", 10m, "cat1", stockQuantity);

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
        var product = Product.Create("Test", "Desc", 10m, "cat1", stockQuantity);

        // Act & Assert
        product.IsLowStock().Should().BeFalse();
    }

    [Fact]
    public void LowStockThreshold_ShouldBe10()
    {
        // Assert
        Product.LowStockThreshold.Should().Be(10);
    }
}
