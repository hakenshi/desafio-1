using HypeSoft.Domain.Entities;
using MongoDB.Driver;

namespace HypeSoft.Infraestructure.Data;

public static class MongoDbIndexes
{
    public static async Task CreateIndexesAsync(MongoDbContext context)
    {
        var productIndexes = new[]
        {
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Text(p => p.Name),
                new CreateIndexOptions { Name = "idx_product_name_text" }
            ),
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(p => p.CategoryId),
                new CreateIndexOptions { Name = "idx_product_category" }
            ),
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys.Ascending(p => p.StockQuantity),
                new CreateIndexOptions { Name = "idx_product_stock" }
            ),
            new CreateIndexModel<Product>(
                Builders<Product>.IndexKeys
                    .Descending(p => p.CreatedAt)
                    .Ascending(p => p.Id),
                new CreateIndexOptions { Name = "idx_product_created_id" }
            )
        };

        await context.Products.Indexes.CreateManyAsync(productIndexes);

        var categoryIndexes = new[]
        {
            new CreateIndexModel<Category>(
                Builders<Category>.IndexKeys.Ascending(c => c.Name),
                new CreateIndexOptions { Name = "idx_category_name", Unique = true }
            )
        };

        await context.Categories.Indexes.CreateManyAsync(categoryIndexes);
    }
}
