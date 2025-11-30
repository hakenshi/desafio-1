using FluentAssertions;
using HypeSoft.Domain.ValueObjects;

namespace HypeSoft.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldCreateMoney()
    {
        // Act
        var money = Money.Create(100.50m);

        // Assert
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowException()
    {
        // Act & Assert
        var act = () => Money.Create(-10);
        act.Should().Throw<ArgumentException>().WithParameterName("amount");
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldAddAmounts()
    {
        // Arrange
        var money1 = Money.Create(100);
        var money2 = Money.Create(50);

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(150);
    }

    [Fact]
    public void Add_WithDifferentCurrency_ShouldThrowException()
    {
        // Arrange
        var money1 = Money.Create(100, "BRL");
        var money2 = Money.Create(50, "USD");

        // Act & Assert
        var act = () => money1.Add(money2);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldSubtractAmounts()
    {
        // Arrange
        var money1 = Money.Create(100);
        var money2 = Money.Create(30);

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(70);
    }

    [Fact]
    public void Multiply_ShouldMultiplyAmount()
    {
        // Arrange
        var money = Money.Create(10);

        // Act
        var result = money.Multiply(5);

        // Assert
        result.Amount.Should().Be(50);
    }

    [Fact]
    public void Zero_ShouldCreateZeroMoney()
    {
        // Act
        var money = Money.Zero();

        // Assert
        money.Amount.Should().Be(0);
    }
}
