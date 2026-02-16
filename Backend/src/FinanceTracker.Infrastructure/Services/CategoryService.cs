using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;
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
    /// <param name="userId">The ID of the user creating the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created category</returns>
    public async Task<Guid> CreateCategoryAsync(CategoryCreateRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating category with name: {CategoryName} for user: {UserId}", request.Name, userId);
        
        var category = MapToEntity(request, userId);

        var createdCategory = await _repository.CreateAsync(category, cancellationToken);
        
        _logger.LogInformation("Category created with ID: {CategoryId} for user: {UserId}", createdCategory.Id, userId);
        
        return createdCategory.Id;
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="request">Category update request</param>
    /// <param name="userId">The ID of the user updating the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if category was found, belongs to user, and was updated; false otherwise</returns>
    public async Task<bool> UpdateCategoryAsync(CategoryUpdateRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating category with ID: {CategoryId} for user: {UserId}", request.Id, userId);
        
        var category = MapToEntity(request, userId);

        var updatedCategory = await _repository.UpdateAsync(category, userId, cancellationToken);
        
        if (updatedCategory == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found or does not belong to user {UserId}", request.Id, userId);
            return false;
        }
        
        _logger.LogInformation("Category updated with ID: {CategoryId} for user: {UserId}", category.Id, userId);
        return true;
    }

    /// <summary>
    /// Gets all categories for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose categories to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all categories for the user as DTOs</returns>
    public async Task<List<CategoryDto>> GetAllCategoriesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all categories for user: {UserId}", userId);
        
        var categories = await _repository.GetAllAsync(userId, cancellationToken);
        
        var categoryDtos = categories.Select(MapToDto).ToList();
        
        _logger.LogInformation("{Count} categories retrieved for user: {UserId}", categoryDtos.Count, userId);
        
        return categoryDtos;
    }

    /// <summary>
    /// Deletes a category if it belongs to the specified user
    /// </summary>
    /// <param name="id">The ID of the category to delete</param>
    /// <param name="userId">The ID of the user who owns the category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if category was found, belongs to user, and was deleted; false otherwise</returns>
    public async Task<bool> DeleteCategoryAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting category with ID: {CategoryId} for user: {UserId}", id, userId);
        
        var deleted = await _repository.DeleteAsync(id, userId, cancellationToken);
        
        if (deleted)
        {
            _logger.LogInformation("Category deleted successfully with ID: {CategoryId} for user: {UserId}", id, userId);
        }
        else
        {
            _logger.LogWarning("Category with ID {CategoryId} not found or does not belong to user {UserId}", id, userId);
        }
        
        return deleted;
    }

    /// <summary>
    /// Maps Category entity to CategoryDto
    /// </summary>
    /// <param name="category">The category entity</param>
    /// <returns>CategoryDto</returns>
    private CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    /// <summary>
    /// Maps CategoryCreateRequest to Category entity
    /// </summary>
    /// <param name="request">The request model</param>
    /// <param name="userId">The ID of the user creating the category</param>
    /// <returns>Category entity</returns>
    private Category MapToEntity(CategoryCreateRequest request, Guid userId)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            UserId = userId
        };
    }

    /// <summary>
    /// Maps CategoryUpdateRequest to Category entity
    /// </summary>
    /// <param name="request">The request model</param>
    /// <param name="userId">The ID of the user updating the category</param>
    /// <returns>Category entity</returns>
    private Category MapToEntity(CategoryUpdateRequest request, Guid userId)
    {
        return new Category
        {
            Id = request.Id,
            Name = request.Name,
            UserId = userId
        };
    }
}
