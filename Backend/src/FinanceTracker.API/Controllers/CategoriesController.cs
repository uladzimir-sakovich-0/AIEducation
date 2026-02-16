using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;
using FinanceTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

/// <summary>
/// Controller for category management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new category
    /// </summary>
    /// <param name="request">Category creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>201 Created with the category ID, or 400 Bad Request for validation errors</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CategoryCreateRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to create category with name: {CategoryName}", request.Name);
        
        var categoryId = await _categoryService.CreateCategoryAsync(request, cancellationToken);
        
        _logger.LogInformation("Category created successfully with ID: {CategoryId}", categoryId);
        
        return CreatedAtAction(nameof(CreateCategory), new { id = categoryId }, categoryId);
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="request">Category update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>200 OK on successful update, or 400 Bad Request for validation errors</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCategory(
        [FromBody] CategoryUpdateRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to update category with ID: {CategoryId}", request.Id);
        
        await _categoryService.UpdateCategoryAsync(request, cancellationToken);
        
        _logger.LogInformation("Category updated successfully with ID: {CategoryId}", request.Id);
        
        return Ok();
    }

    /// <summary>
    /// Gets all categories
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>200 OK with list of categories</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to get all categories");
        
        var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
        
        _logger.LogInformation("Returning {Count} categories", categories.Count);
        
        return Ok(categories);
    }

    /// <summary>
    /// Deletes a category
    /// </summary>
    /// <param name="id">The ID of the category to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>200 OK when deleted, or 400 Bad Request if not found</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to delete category with ID: {CategoryId}", id);
        
        var deleted = await _categoryService.DeleteCategoryAsync(id, cancellationToken);
        
        if (!deleted)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", id);
            return BadRequest(new ProblemDetails
            {
                Title = "Category Not Found",
                Detail = $"Category with ID {id} does not exist.",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        _logger.LogInformation("Category deleted successfully with ID: {CategoryId}", id);
        
        return Ok();
    }
}
