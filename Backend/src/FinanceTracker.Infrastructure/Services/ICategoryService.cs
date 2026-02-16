using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;

namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Service interface for category business logic
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="request">Category creation request</param>
    /// <param name="userId">The ID of the user creating the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created category</returns>
    Task<Guid> CreateCategoryAsync(CategoryCreateRequest request, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="request">Category update request</param>
    /// <param name="userId">The ID of the user updating the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if category was found, belongs to user, and was updated; false otherwise</returns>
    Task<bool> UpdateCategoryAsync(CategoryUpdateRequest request, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all categories for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose categories to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all categories for the user as DTOs</returns>
    Task<List<CategoryDto>> GetAllCategoriesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category if it belongs to the specified user
    /// </summary>
    /// <param name="id">The ID of the category to delete</param>
    /// <param name="userId">The ID of the user who owns the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if category was found, belongs to user, and was deleted; false otherwise</returns>
    Task<bool> DeleteCategoryAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
