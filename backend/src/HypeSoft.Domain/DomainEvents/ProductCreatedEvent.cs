namespace HypeSoft.Domain.DomainEvents;

/// <summary>
/// Event raised when a new product is created
/// </summary>
public record ProductCreatedEvent : IDomainEvent
{
    public string ProductId { get; }
    public string ProductName { get; }
    public string CategoryId { get; }
    public DateTime OccurredOn { get; }

    public ProductCreatedEvent(string productId, string productName, string categoryId)
    {
        ProductId = productId;
        ProductName = productName;
        CategoryId = categoryId;
        OccurredOn = DateTime.UtcNow;
    }
}
