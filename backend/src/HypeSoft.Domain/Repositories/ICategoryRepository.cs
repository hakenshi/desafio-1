using HypeSoft.Domain.Entities;

namespace HypeSoft.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<Dictionary<string, int>> GetProductCountByCategoryAsync(CancellationToken cancellationToken = default);
}
