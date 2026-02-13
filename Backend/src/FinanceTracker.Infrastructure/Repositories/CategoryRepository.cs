using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Category entity operations
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly FinanceTrackerDbContext _context;
    private readonly ILogger<CategoryRepository> _logger;

    public CategoryRepository(FinanceTrackerDbContext context, ILogger<CategoryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new category in the database
    /// </summary>
    /// <param name="category">The category to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created category with generated ID</returns>
    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new category with name: {CategoryName}", category.Name);
        
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Category created successfully with ID: {CategoryId}", category.Id);
        
        return category;
    }
}
