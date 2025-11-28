using HypeSoft.Domain.Entities;
using HypeSoft.Domain.Repositories;
using HypeSoft.Infraestructure.Data;
using MongoDB.Driver;

namespace HypeSoft.Infraestructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly MongoDbContext _context;

    public CategoryRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Find(c => c.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Find(_ => true)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.InsertOneAsync(category, cancellationToken: cancellationToken);
        return category;
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.ReplaceOneAsync(
            c => c.Id == category.Id,
            category,
            cancellationToken: cancellationToken);
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await _context.Categories.DeleteOneAsync(c => c.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<Dictionary<string, int>> GetProductCountByCategoryAsync(CancellationToken cancellationToken = default)
    {
        var products = await _context.Products.Find(_ => true).ToListAsync(cancellationToken);
        var categories = await _context.Categories.Find(_ => true).ToListAsync(cancellationToken);
        
        return categories.ToDictionary(
            c => c.Name,
            c => products.Count(p => p.CategoryId == c.Id)
        );
    }
}
