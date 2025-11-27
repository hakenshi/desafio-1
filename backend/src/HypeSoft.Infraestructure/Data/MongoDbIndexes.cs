using HypeSoft.Domain.Entities;
using MongoDB.Driver;

namespace HypeSoft.Infraestructure.Data;

public static class MongoDbIndexes
{
    public static async Task CreateIndexesAsync(MongoDbContext context)
    {
        // Product indexes
        var productIndexes = new[]
        {
            // Index for name search
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Text(p => p.Name),
                new CreateIndexOptions { Name = "idx_product_name_text" }
            ),
            // Index for category filtering
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(p => p.CategoryId),
                new CreateIndexOptions { Name = "idx_product_category" }
            ),
            // Index for low stock queries
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(p => p.StockQuantity),
                new CreateIndexOptions { Name = "idx_product_stock" }
            ),
            // Compound index for pagination
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys
                    .Descending(p => p.CreatedAt)
                    .Ascending(p => p.Id),
                new CreateIndexOptions { Name = "idx_product_created_id" }
            )
        };

        await context.Products.Indexes.CreateManyAsync(productIndexes);

        // Category indexes
        var categoryIndexes = new[]
        {
            // Index for category name
            new CreateIndexModel<Category>(
                Builders<Category>.IndexKeys.Ascending(c => c.Name),
                new CreateIndexOptions { Name = "idx_category_name", Unique = true }
            )
        };

        await context.Categories.Indexes.CreateManyAsync(categoryIndexes);
    }
}
