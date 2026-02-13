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

    /// <summary>
    /// Updates an existing category in the database
    /// </summary>
    /// <param name="category">The category to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated category</returns>
    public async Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating category with ID: {CategoryId}", category.Id);
        
        // Check if the entity is already being tracked
        var trackedEntity = _context.Categories.Local.FirstOrDefault(c => c.Id == category.Id);
        if (trackedEntity != null)
        {
            // If already tracked, update its properties
            _context.Entry(trackedEntity).CurrentValues.SetValues(category);
        }
        else
        {
            // If not tracked, update the entity
            _context.Categories.Update(category);
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Category updated successfully with ID: {CategoryId}", category.Id);
        
        return trackedEntity ?? category;
    }
}
