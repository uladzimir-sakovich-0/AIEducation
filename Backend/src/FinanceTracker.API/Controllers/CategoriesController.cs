using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;
using FinanceTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to create category with name: {CategoryName} for user: {UserId}", request.Name, userId);
        
        var categoryId = await _categoryService.CreateCategoryAsync(request, userId, cancellationToken);
        
        _logger.LogInformation("Category created successfully with ID: {CategoryId} for user: {UserId}", categoryId, userId);
        
        return CreatedAtAction(nameof(CreateCategory), new { id = categoryId }, categoryId);
    }

    /// <summary>
    /// Updates an existing category
    /// </summary>
    /// <param name="request">Category update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>200 OK on successful update, or 400 Bad Request if category not found or doesn't belong to user</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCategory(
        [FromBody] CategoryUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to update category with ID: {CategoryId} for user: {UserId}", request.Id, userId);
        
        var updated = await _categoryService.UpdateCategoryAsync(request, userId, cancellationToken);
        
        if (!updated)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found or does not belong to user {UserId}", request.Id, userId);
            return BadRequest(new ProblemDetails
            {
                Title = "Category Not Found",
                Detail = $"Category with ID {request.Id} does not exist or you do not have permission to update it.",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        _logger.LogInformation("Category updated successfully with ID: {CategoryId} for user: {UserId}", request.Id, userId);
        
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
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to get all categories for user: {UserId}", userId);
        
        var categories = await _categoryService.GetAllCategoriesAsync(userId, cancellationToken);
        
        _logger.LogInformation("Returning {Count} categories for user: {UserId}", categories.Count, userId);
        
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
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to delete category with ID: {CategoryId} for user: {UserId}", id, userId);
        
        var deleted = await _categoryService.DeleteCategoryAsync(id, userId, cancellationToken);
        
        if (!deleted)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found or does not belong to user {UserId}", id, userId);
            return BadRequest(new ProblemDetails
            {
                Title = "Category Not Found",
                Detail = $"Category with ID {id} does not exist or you do not have permission to delete it.",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        _logger.LogInformation("Category deleted successfully with ID: {CategoryId} for user: {UserId}", id, userId);
        
        return Ok();
    }

    /// <summary>
    /// Extracts the user ID from JWT claims
    /// </summary>
    /// <returns>The user ID as a Guid</returns>
    private Guid GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }
        
        return userId;
    }
}
