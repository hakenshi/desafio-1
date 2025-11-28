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
            Category.Create("Eletrônicos", "Produtos eletrônicos e tecnologia"),
            Category.Create("Roupas", "Vestuário e acessórios"),
            Category.Create("Alimentos", "Produtos alimentícios"),
            Category.Create("Livros", "Livros e publicações"),
            Category.Create("Móveis", "Móveis e decoração")
        };

        await context.Categories.InsertManyAsync(categories);

        // Seed Products
        var products = new List<Product>
        {
            Product.Create("Notebook Dell", "Notebook Dell Inspiron 15", 3500.00m, categories[0].Id, 15),
            Product.Create("Mouse Logitech", "Mouse sem fio Logitech MX Master", 350.00m, categories[0].Id, 5),
            Product.Create("Teclado Mecânico", "Teclado mecânico RGB", 450.00m, categories[0].Id, 8),
            Product.Create("Camiseta Básica", "Camiseta 100% algodão", 49.90m, categories[1].Id, 50),
            Product.Create("Calça Jeans", "Calça jeans slim fit", 129.90m, categories[1].Id, 3),
            Product.Create("Café Premium", "Café torrado e moído 500g", 25.00m, categories[2].Id, 100),
            Product.Create("Clean Code", "Livro Clean Code - Robert Martin", 89.90m, categories[3].Id, 7),
            Product.Create("Cadeira Gamer", "Cadeira gamer ergonômica", 1200.00m, categories[4].Id, 2)
        };

        await context.Products.InsertManyAsync(products);
    }
}
