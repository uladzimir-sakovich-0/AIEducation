using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Models.Requests;
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
    /// <param name="request">Category creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created category</returns>
    public async Task<Guid> CreateCategoryAsync(CategoryCreateRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating category with name: {CategoryName}", request.Name);
        
        var category = MapToEntity(request);

        var createdCategory = await _repository.CreateAsync(category, cancellationToken);
        
        _logger.LogInformation("Category created with ID: {CategoryId}", createdCategory.Id);
        
        return createdCategory.Id;
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="request">Category update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    public async Task UpdateCategoryAsync(CategoryUpdateRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating category with ID: {CategoryId}", request.Id);
        
        var category = MapToEntity(request);

        await _repository.UpdateAsync(category, cancellationToken);
        
        _logger.LogInformation("Category updated with ID: {CategoryId}", category.Id);
    }

    /// <summary>
    /// Maps CategoryCreateRequest to Category entity
    /// </summary>
    /// <param name="request">The request model</param>
    /// <returns>Category entity</returns>
    private Category MapToEntity(CategoryCreateRequest request)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };
    }

    /// <summary>
    /// Maps CategoryUpdateRequest to Category entity
    /// </summary>
    /// <param name="request">The request model</param>
    /// <returns>Category entity</returns>
    private Category MapToEntity(CategoryUpdateRequest request)
    {
        return new Category
        {
            Id = request.Id,
            Name = request.Name
        };
    }
}
