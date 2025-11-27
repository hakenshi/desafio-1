using HypeSoft.Domain.Entities;
using HypeSoft.Infraestructure.Data;
using MongoDB.Driver;

namespace HypeSoft.API.Extensions;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(MongoDbContext context)
    {
        // Check if data already exists
        var existingCategories = await context.Categories.Find(_ => true).AnyAsync();
        if (existingCategories) return;

        // Seed Categories
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid().ToString(), Name = "Eletrônicos", Description = "Produtos eletrônicos e tecnologia", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid().ToString(), Name = "Roupas", Description = "Vestuário e acessórios", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid().ToString(), Name = "Alimentos", Description = "Produtos alimentícios", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid().ToString(), Name = "Livros", Description = "Livros e publicações", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid().ToString(), Name = "Móveis", Description = "Móveis e decoração", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        await context.Categories.InsertManyAsync(categories);

        // Seed Products
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid().ToString(), Name = "Notebook Dell", Description = "Notebook Dell Inspiron 15", Price = 3500.00m, CategoryId = categories[0].Id, StockQuantity = 15, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid().ToString(), Name = "Mouse Logitech", Description = "Mouse sem fio Logitech MX Master", Price = 350.00m, CategoryId = categories[0].Id, StockQuantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid().ToString(), Name = "Teclado Mecânico", Description = "Teclado mecânico RGB", Price = 450.00m, CategoryId = categories[0].Id, StockQuantity = 8, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid().ToString(), Name = "Camiseta Básica", Description = "Camiseta 100% algodão", Price = 49.90m, CategoryId = categories[1].Id, StockQuantity = 50, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid().ToString(), Name = "Calça Jeans", Description = "Calça jeans slim fit", Price = 129.90m, CategoryId = categories[1].Id, StockQuantity = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid().ToString(), Name = "Café Premium", Description = "Café torrado e moído 500g", Price = 25.00m, CategoryId = categories[2].Id, StockQuantity = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid().ToString(), Name = "Clean Code", Description = "Livro Clean Code - Robert Martin", Price = 89.90m, CategoryId = categories[3].Id, StockQuantity = 7, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid().ToString(), Name = "Cadeira Gamer", Description = "Cadeira gamer ergonômica", Price = 1200.00m, CategoryId = categories[4].Id, StockQuantity = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        await context.Products.InsertManyAsync(products);
    }
}
