namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Service interface for category business logic
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="name">Name of the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created category</returns>
    Task<Guid> CreateCategoryAsync(string name, CancellationToken cancellationToken = default);
}
