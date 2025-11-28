namespace HypeSoft.Domain.Entities;

public class Product
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string CategoryId { get; set; } = null!;
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public bool IsLowStock() => StockQuantity < 10;

    public static Product Create(string name, string description, decimal price, string categoryId, int stockQuantity)
    {
        return new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = description,
            Price = price,
            CategoryId = categoryId,
            StockQuantity = stockQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
