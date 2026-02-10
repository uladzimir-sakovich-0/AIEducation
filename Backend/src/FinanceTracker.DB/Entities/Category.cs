using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.DB.Entities;

/// <summary>
/// Represents a category for transactions
/// </summary>
public class Category
{
    /// <summary>
    /// Unique identifier for the category
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the category
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property for transactions in this category
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
