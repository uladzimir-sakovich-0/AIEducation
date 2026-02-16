using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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
    /// Updates an existing category in the database if it belongs to the specified user
    /// </summary>
    /// <param name="category">The category to update</param>
    /// <param name="userId">The ID of the user who owns the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated category, or null if not found or doesn't belong to user</returns>
    public async Task<Category?> UpdateAsync(Category category, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating category with ID: {CategoryId} for user: {UserId}", category.Id, userId);
        
        // First verify the category exists and belongs to the user
        var existingCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == category.Id && c.UserId == userId, cancellationToken);
        
        if (existingCategory == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found or does not belong to user {UserId}", category.Id, userId);
            return null;
        }
        
        // Update only the allowed properties (Name in this case)
        existingCategory.Name = category.Name;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Category updated successfully with ID: {CategoryId} for user: {UserId}", category.Id, userId);
        
        return existingCategory;
    }

    /// <summary>
    /// Gets all categories from the database for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose categories to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all categories for the user</returns>
    public async Task<List<Category>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all categories from database for user: {UserId}", userId);
        
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} categories from database for user: {UserId}", categories.Count, userId);
        
        return categories;
    }

    /// <summary>
    /// Deletes a category from the database if it belongs to the specified user
    /// </summary>
    /// <param name="id">The ID of the category to delete</param>
    /// <param name="userId">The ID of the user who owns the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if category was found, belongs to user, and was deleted; false otherwise</returns>
    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete category with ID: {CategoryId} for user: {UserId}", id, userId);
        
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, cancellationToken);
        
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found or does not belong to user {UserId}", id, userId);
            return false;
        }
        
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Category with ID {CategoryId} deleted successfully for user: {UserId}", id, userId);
        
        return true;
    }
}
