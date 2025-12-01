using HypeSoft.Domain.Entities;
using HypeSoft.Infraestructure.Data;
using MongoDB.Driver;

namespace HypeSoft.API.Extensions;

public static class DatabaseSeeder
{
    private static readonly Random _random = new();

    public static async Task SeedAsync(MongoDbContext context)
    {
        // Check if data already exists
        var existingCategories = await context.Categories.Find(_ => true).AnyAsync();
        if (existingCategories) return;

        Console.WriteLine("Starting database seeding with 10,000 products...");

        // Seed Categories (50 categories)
        var categories = CreateCategories();
        await context.Categories.InsertManyAsync(categories);
        Console.WriteLine($"Created {categories.Count} categories");

        // Seed Products (10,000 products in batches)
        var products = CreateProducts(categories, 10000);
        
        // Insert in batches of 1000 for better performance
        const int batchSize = 1000;
        for (int i = 0; i < products.Count; i += batchSize)
        {
            var batch = products.Skip(i).Take(batchSize).ToList();
            await context.Products.InsertManyAsync(batch);
            Console.WriteLine($"Inserted products {i + 1} to {Math.Min(i + batchSize, products.Count)}");
        }

        Console.WriteLine($"Database seeding completed! Total: {products.Count} products");
    }

    private static List<Category> CreateCategories()
    {
        return new List<Category>
        {
            // Electronics
            Category.Create("Smartphones", "Mobile phones and accessories"),
            Category.Create("Laptops", "Notebooks and ultrabooks"),
            Category.Create("Tablets", "Tablets and e-readers"),
            Category.Create("Audio", "Headphones, speakers and audio equipment"),
            Category.Create("Gaming", "Gaming consoles and accessories"),
            Category.Create("Cameras", "Digital cameras and photography equipment"),
            Category.Create("Wearables", "Smartwatches and fitness trackers"),
            Category.Create("Computer Parts", "PC components and peripherals"),
            Category.Create("TVs", "Televisions and home theater"),
            Category.Create("Smart Home", "Home automation devices"),
            
            // Fashion
            Category.Create("Men's Clothing", "Clothing for men"),
            Category.Create("Women's Clothing", "Clothing for women"),
            Category.Create("Kids' Clothing", "Clothing for children"),
            Category.Create("Shoes", "Footwear for all ages"),
            Category.Create("Accessories", "Bags, belts, and fashion accessories"),
            Category.Create("Jewelry", "Rings, necklaces, and jewelry"),
            Category.Create("Watches", "Wristwatches and luxury timepieces"),
            Category.Create("Sportswear", "Athletic and sports clothing"),
            
            // Home & Garden
            Category.Create("Furniture", "Home and office furniture"),
            Category.Create("Kitchen", "Kitchen appliances and utensils"),
            Category.Create("Bedding", "Bed sheets, pillows, and mattresses"),
            Category.Create("Bathroom", "Bathroom accessories and fixtures"),
            Category.Create("Garden", "Gardening tools and outdoor furniture"),
            Category.Create("Lighting", "Lamps and lighting fixtures"),
            Category.Create("Decor", "Home decoration items"),
            
            // Food & Beverages
            Category.Create("Groceries", "Daily grocery items"),
            Category.Create("Beverages", "Drinks and beverages"),
            Category.Create("Snacks", "Snacks and confectionery"),
            Category.Create("Organic", "Organic and natural products"),
            Category.Create("Gourmet", "Premium and gourmet foods"),
            
            // Books & Media
            Category.Create("Fiction", "Fiction books and novels"),
            Category.Create("Non-Fiction", "Non-fiction and educational books"),
            Category.Create("Tech Books", "Programming and technology books"),
            Category.Create("Comics", "Comics and graphic novels"),
            Category.Create("Music", "CDs, vinyl, and music merchandise"),
            Category.Create("Movies", "DVDs, Blu-rays, and streaming"),
            
            // Sports & Outdoors
            Category.Create("Fitness", "Gym equipment and fitness gear"),
            Category.Create("Cycling", "Bikes and cycling accessories"),
            Category.Create("Camping", "Camping and hiking gear"),
            Category.Create("Water Sports", "Swimming and water sports equipment"),
            Category.Create("Team Sports", "Equipment for team sports"),
            
            // Health & Beauty
            Category.Create("Skincare", "Skincare products"),
            Category.Create("Haircare", "Hair products and styling tools"),
            Category.Create("Makeup", "Cosmetics and makeup"),
            Category.Create("Fragrances", "Perfumes and colognes"),
            Category.Create("Health", "Health supplements and vitamins"),
            
            // Automotive
            Category.Create("Car Parts", "Auto parts and accessories"),
            Category.Create("Car Electronics", "Car audio and electronics"),
            Category.Create("Tools", "Automotive tools"),
            Category.Create("Motorcycle", "Motorcycle parts and gear")
        };
    }

    private static List<Product> CreateProducts(List<Category> categories, int count)
    {
        var products = new List<Product>(count);
        var productTemplates = GetProductTemplates();

        for (int i = 1; i <= count; i++)
        {
            var category = categories[_random.Next(categories.Count)];
            var template = productTemplates[_random.Next(productTemplates.Count)];
            
            var name = $"{template.Adjective} {template.ProductType} {template.Variant} #{i}";
            var description = $"{template.Description} - SKU: PRD{i:D6}";
            var price = Math.Round((decimal)(_random.NextDouble() * 4990 + 10), 2); // $10 to $5000
            var stock = _random.Next(0, 500); // 0 to 500 units

            products.Add(Product.Create(name, description, price, category.Id, stock));
        }

        return products;
    }

    private static List<ProductTemplate> GetProductTemplates()
    {
        return new List<ProductTemplate>
        {
            new("Premium", "Laptop", "Pro", "High-performance laptop for professionals"),
            new("Ultra", "Smartphone", "Max", "Latest generation smartphone with advanced features"),
            new("Classic", "Watch", "Edition", "Elegant timepiece with precision movement"),
            new("Sport", "Headphones", "Wireless", "Noise-canceling wireless headphones"),
            new("Deluxe", "Camera", "4K", "Professional-grade digital camera"),
            new("Essential", "Tablet", "Lite", "Lightweight tablet for everyday use"),
            new("Pro", "Monitor", "Curved", "Ultra-wide curved gaming monitor"),
            new("Elite", "Keyboard", "Mechanical", "RGB mechanical keyboard for gaming"),
            new("Comfort", "Chair", "Ergonomic", "Ergonomic office chair with lumbar support"),
            new("Smart", "Speaker", "Voice", "Voice-controlled smart speaker"),
            new("Vintage", "Jacket", "Leather", "Classic leather jacket with modern fit"),
            new("Athletic", "Sneakers", "Running", "Lightweight running shoes with cushioning"),
            new("Organic", "Coffee", "Blend", "Premium organic coffee beans"),
            new("Bestseller", "Book", "Hardcover", "Award-winning bestseller book"),
            new("Professional", "Tool", "Set", "Complete professional tool set"),
            new("Luxury", "Perfume", "Collection", "Exclusive fragrance collection"),
            new("Natural", "Skincare", "Serum", "Natural ingredients skincare serum"),
            new("Gaming", "Console", "Bundle", "Gaming console with accessories bundle"),
            new("Portable", "Charger", "Fast", "Fast-charging portable power bank"),
            new("Wireless", "Mouse", "Ergonomic", "Ergonomic wireless mouse"),
            new("HD", "TV", "Smart", "Smart TV with 4K resolution"),
            new("Compact", "Drone", "Camera", "Compact drone with HD camera"),
            new("Fitness", "Tracker", "Band", "Fitness tracker with heart rate monitor"),
            new("Gourmet", "Chocolate", "Box", "Premium gourmet chocolate selection"),
            new("Designer", "Bag", "Leather", "Designer leather handbag"),
            new("Acoustic", "Guitar", "Steel", "Steel string acoustic guitar"),
            new("Electric", "Scooter", "Foldable", "Foldable electric scooter"),
            new("Memory", "Foam", "Pillow", "Memory foam ergonomic pillow"),
            new("Stainless", "Cookware", "Set", "Stainless steel cookware set"),
            new("LED", "Light", "Strip", "RGB LED light strip with remote")
        };
    }

    private record ProductTemplate(string Adjective, string ProductType, string Variant, string Description);
}
