namespace HypeSoft.Domain.ValueObjects;

/// <summary>
/// Value Object representing stock quantity with low stock threshold
/// </summary>
public record StockQuantity
{
    public int Quantity { get; }
    public const int LowStockThreshold = 10;

    private StockQuantity(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));

        Quantity = quantity;
    }

    public static StockQuantity Create(int quantity) => new(quantity);

    public static StockQuantity Zero => new(0);

    public bool IsLowStock => Quantity < LowStockThreshold;

    public bool IsOutOfStock => Quantity == 0;

    public StockQuantity Add(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Cannot add negative quantity", nameof(quantity));
        
        return new StockQuantity(Quantity + quantity);
    }

    public StockQuantity Subtract(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Cannot subtract negative quantity", nameof(quantity));
        
        if (quantity > Quantity)
            throw new InvalidOperationException("Insufficient stock");
        
        return new StockQuantity(Quantity - quantity);
    }

    public static implicit operator int(StockQuantity stock) => stock.Quantity;

    public override string ToString() => Quantity.ToString();
}
