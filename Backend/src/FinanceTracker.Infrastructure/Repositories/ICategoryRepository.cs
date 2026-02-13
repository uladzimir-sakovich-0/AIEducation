using FinanceTracker.DB.Entities;

namespace FinanceTracker.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Category entity operations
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// Creates a new category in the database
    /// </summary>
    /// <param name="category">The category to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created category with generated ID</returns>
    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category in the database
    /// </summary>
    /// <param name="category">The category to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated category</returns>
    Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default);
}
