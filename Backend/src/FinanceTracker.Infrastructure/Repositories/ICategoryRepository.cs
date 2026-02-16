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
    /// Updates an existing category in the database if it belongs to the specified user
    /// </summary>
    /// <param name="category">The category to update</param>
    /// <param name="userId">The ID of the user who owns the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated category, or null if not found or doesn't belong to user</returns>
    Task<Category?> UpdateAsync(Category category, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all categories from the database for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose categories to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all categories for the user</returns>
    Task<List<Category>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category from the database if it belongs to the specified user
    /// </summary>
    /// <param name="id">The ID of the category to delete</param>
    /// <param name="userId">The ID of the user who owns the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if category was found, belongs to user, and was deleted; false otherwise</returns>
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
