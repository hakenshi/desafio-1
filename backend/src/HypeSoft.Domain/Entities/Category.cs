namespace HypeSoft.Domain.Entities;

public class Category
{
    public string Id { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Category() { }

    public static Category Create(string name, string description)
    {
        ValidateCategoryData(name, description);
        
        return new Category
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string description)
    {
        ValidateCategoryData(name, description);
        
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateCategoryData(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));
        
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Category description cannot be empty", nameof(description));
    }
}
