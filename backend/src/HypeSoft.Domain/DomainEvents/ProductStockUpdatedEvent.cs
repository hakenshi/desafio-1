namespace HypeSoft.Domain.DomainEvents;

/// <summary>
/// Event raised when product stock is updated
/// </summary>
public record ProductStockUpdatedEvent : IDomainEvent
{
    public string ProductId { get; }
    public int OldQuantity { get; }
    public int NewQuantity { get; }
    public bool IsLowStock { get; }
    public DateTime OccurredOn { get; }

    public ProductStockUpdatedEvent(string productId, int oldQuantity, int newQuantity)
    {
        ProductId = productId;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
        IsLowStock = newQuantity < 10;
        OccurredOn = DateTime.UtcNow;
    }
}
