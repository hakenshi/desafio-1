namespace HypeSoft.Domain.DomainEvents;

/// <summary>
/// Event raised when a product reaches low stock threshold
/// </summary>
public record LowStockAlertEvent : IDomainEvent
{
    public string ProductId { get; }
    public string ProductName { get; }
    public int CurrentQuantity { get; }
    public DateTime OccurredOn { get; }

    public LowStockAlertEvent(string productId, string productName, int currentQuantity)
    {
        ProductId = productId;
        ProductName = productName;
        CurrentQuantity = currentQuantity;
        OccurredOn = DateTime.UtcNow;
    }
}
