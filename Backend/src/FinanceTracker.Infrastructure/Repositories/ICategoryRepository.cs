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

    /// <summary>
    /// Gets all categories from the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all categories</returns>
    Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category from the database
    /// </summary>
    /// <param name="id">The ID of the category to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if category was found and deleted, false if not found</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
