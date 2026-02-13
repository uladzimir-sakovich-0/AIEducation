namespace FinanceTracker.Infrastructure.Models.Requests;

/// <summary>
/// Request model for creating a new category
/// </summary>
public class CategoryCreateRequest
{
    /// <summary>
    /// Name of the category
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
