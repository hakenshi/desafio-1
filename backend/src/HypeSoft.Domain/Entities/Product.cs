namespace HypeSoft.Domain.Entities;

public class Product
{
    public const int LowStockThreshold = 10;
    private static int _skuCounter = 1;
    
    public string Id { get; private set; } = null!;
    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public decimal Price { get; private set; }
    public string CategoryId { get; private set; } = null!;
    public int StockQuantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Parameterless constructor for serialization
    private Product() { }
    
    public bool IsLowStock() => StockQuantity < LowStockThreshold;

    private static string GenerateSku()
    {
        return $"PRD{_skuCounter++:D6}";
    }

    public static Product Create(string name, string description, decimal price, string categoryId, int stockQuantity)
    {
        ValidateProductData(name, description, price, stockQuantity);
        
        return new Product
        {
            Id = Guid.NewGuid().ToString(),
            Sku = GenerateSku(),
            Name = name,
            Description = description,
            Price = price,
            CategoryId = categoryId,
            StockQuantity = stockQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description, decimal price, string categoryId, int stockQuantity)
    {
        ValidateProductData(name, description, price, stockQuantity);
        
        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        StockQuantity = stockQuantity;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateProductData(string name, string description, decimal price, int stockQuantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));
        
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description cannot be empty", nameof(description));
        
        if (price < 0)
            throw new ArgumentException("Product price cannot be negative", nameof(price));
        
        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));
    }
}
