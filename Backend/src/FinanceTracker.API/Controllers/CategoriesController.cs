using FinanceTracker.API.Models.Requests;
using FinanceTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

/// <summary>
/// Controller for category management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
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
        
        var categoryId = await _categoryService.CreateCategoryAsync(request.Name, cancellationToken);
        
        _logger.LogInformation("Category created successfully with ID: {CategoryId}", categoryId);
        
        return CreatedAtAction(nameof(CreateCategory), new { id = categoryId }, categoryId);
    }
}
