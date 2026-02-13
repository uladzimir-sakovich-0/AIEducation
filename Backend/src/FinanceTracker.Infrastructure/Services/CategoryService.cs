using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Service implementation for category business logic
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="name">Name of the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created category</returns>
    public async Task<Guid> CreateCategoryAsync(string name, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating category with name: {CategoryName}", name);
        
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        var createdCategory = await _repository.CreateAsync(category, cancellationToken);
        
        _logger.LogInformation("Category created with ID: {CategoryId}", createdCategory.Id);
        
        return createdCategory.Id;
    }
}
