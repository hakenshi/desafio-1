using FluentAssertions;
using HypeSoft.Domain.Entities;

namespace HypeSoft.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_ValidData_ShouldCreateProduct()
    {
        var product = Product.Create("Test Product", "Test Description", 100m, "cat1", 5);

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
        var act = () => Product.Create("", "Description", 100m, "cat1", 5);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*name*");
    }

    [Fact]
    public void Create_EmptyDescription_ShouldThrowArgumentException()
    {
        var act = () => Product.Create("Name", "", 100m, "cat1", 5);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*description*");
    }

    [Fact]
    public void Create_NegativePrice_ShouldThrowArgumentException()
    {
        var act = () => Product.Create("Name", "Description", -10m, "cat1", 5);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*price*");
    }

    [Fact]
    public void Create_NegativeStock_ShouldThrowArgumentException()
    {
        var act = () => Product.Create("Name", "Description", 100m, "cat1", -5);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Stock*");
    }

    [Fact]
    public void Update_ValidData_ShouldUpdateProduct()
    {
        var product = Product.Create("Original", "Original Desc", 50m, "cat1", 10);
        var originalCreatedAt = product.CreatedAt;

        product.Update("Updated", "Updated Desc", 100m, "cat2", 20);

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
        var product = Product.Create("Name", "Description", 100m, "cat1", 5);

        var act = () => product.Update("", "Description", 100m, "cat1", 5);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*name*");
    }

    [Fact]
    public void IsLowStock_WhenStockIsLessThanThreshold_ShouldReturnTrue()
    {
        var product = Product.Create("Test Product", "Test Description", 100m, "cat1", 5);

        var result = product.IsLowStock();

        result.Should().BeTrue();
    }

    [Fact]
    public void IsLowStock_WhenStockIsEqualOrAboveThreshold_ShouldReturnFalse()
    {
        var product = Product.Create("Test Product", "Test Description", 100m, "cat1", 10);

        var result = product.IsLowStock();

        result.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(9)]
    public void IsLowStock_WithVariousLowStockValues_ShouldReturnTrue(int stockQuantity)
    {
        var product = Product.Create("Test", "Desc", 10m, "cat1", stockQuantity);

        product.IsLowStock().Should().BeTrue();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void IsLowStock_WithVariousHighStockValues_ShouldReturnFalse(int stockQuantity)
    {
        var product = Product.Create("Test", "Desc", 10m, "cat1", stockQuantity);

        product.IsLowStock().Should().BeFalse();
    }

    [Fact]
    public void LowStockThreshold_ShouldBe10()
    {
        Product.LowStockThreshold.Should().Be(10);
    }
}
