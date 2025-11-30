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

    [Fact]
    public void Create_WithEmptyName_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Category.Create("", "Valid Description");
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void Create_WithWhitespaceName_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Category.Create(" ", "Valid Description");
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Category.Create(null!, "Valid Description");
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void Create_WithEmptyDescription_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Category.Create("Valid Name", "");
        act.Should().Throw<ArgumentException>().WithParameterName("description");
    }

    [Fact]
    public void Create_WithWhitespaceDescription_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Category.Create("Valid Name", " ");
        act.Should().Throw<ArgumentException>().WithParameterName("description");
    }

    [Fact]
    public void Create_WithNullDescription_ShouldThrowArgumentException()
    {
        // Act & Assert
        var act = () => Category.Create("Valid Name", null!);
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
