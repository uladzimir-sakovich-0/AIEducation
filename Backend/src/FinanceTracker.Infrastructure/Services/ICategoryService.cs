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
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created category</returns>
    Task<Guid> CreateCategoryAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="request">Category update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateCategoryAsync(CategoryUpdateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all categories
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all categories as DTOs</returns>
    Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category
    /// </summary>
    /// <param name="id">The ID of the category to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if category was found and deleted, false if not found</returns>
    Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
}
