using FluentAssertions;
using HypeSoft.Domain.Entities;

namespace HypeSoft.UnitTests.Domain;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCategory()
    {
        // Arrange
        var name = "Electronics";
        var description = "Electronic products and technology";

        // Act
        var category = Category.Create(name, description);

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(name);
        category.Description.Should().Be(description);
        category.Id.Should().NotBeNullOrEmpty();
        category.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        category.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Act & Assert
        var act = () => Category.Create(invalidName, "Valid Description");
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidDescription_ShouldThrowArgumentException(string invalidDescription)
    {
        // Act & Assert
        var act = () => Category.Create("Valid Name", invalidDescription);
        act.Should().Throw<ArgumentException>().WithParameterName("description");
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateCategory()
    {
        // Arrange
        var category = Category.Create("Original Name", "Original Description");
        var originalCreatedAt = category.CreatedAt;
        
        // Wait a bit to ensure UpdatedAt changes
        Thread.Sleep(10);

        // Act
        category.Update("Updated Name", "Updated Description");

        // Assert
        category.Name.Should().Be("Updated Name");
        category.Description.Should().Be("Updated Description");
        category.CreatedAt.Should().Be(originalCreatedAt); // Should not change
        category.UpdatedAt.Should().BeAfter(originalCreatedAt);
    }

    [Fact]
    public void Update_WithInvalidData_ShouldThrowArgumentException()
    {
        // Arrange
        var category = Category.Create("Valid Name", "Valid Description");

        // Act & Assert
        var act = () => category.Update("", "Valid Description");
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }
}
