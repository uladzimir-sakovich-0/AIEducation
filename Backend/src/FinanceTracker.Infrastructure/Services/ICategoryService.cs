using FinanceTracker.Infrastructure.Models.Requests;

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
}
