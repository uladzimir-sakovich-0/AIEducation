namespace FinanceTracker.Infrastructure.Models.Responses;

/// <summary>
/// Data transfer object for Category
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// Unique identifier for the category
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the category
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
