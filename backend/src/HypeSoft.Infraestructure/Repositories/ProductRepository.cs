using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using HypeSoft.Infraestructure.Data;
using MongoDB.Driver;

namespace HypeSoft.Infraestructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly MongoDbContext _context;

    public ProductRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Find(_ => true)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(name, "i"));
        return await _context.Products
            .Find(filter)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(string categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Find(p => p.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Find(p => p.StockQuantity < Product.LowStockThreshold)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.InsertOneAsync(product, cancellationToken: cancellationToken);
        return product;
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.ReplaceOneAsync(
            p => p.Id == product.Id,
            product,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _context.Products.DeleteOneAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
    {
        return (int)await _context.Products.CountDocumentsAsync(_ => true, cancellationToken: cancellationToken);
    }

    public async Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default)
    {
        var products = await _context.Products.Find(_ => true).ToListAsync(cancellationToken);
        return products.Sum(p => p.Price * p.StockQuantity);
    }
}
