namespace FinanceTracker.Infrastructure.Models.Requests;

/// <summary>
/// Request model for updating an existing category
/// </summary>
public class CategoryUpdateRequest
{
    /// <summary>
    /// ID of the category to update
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the category
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
