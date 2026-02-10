using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracker.DB.Entities;

/// <summary>
/// Represents a financial transaction
/// </summary>
public class Transaction
{
    /// <summary>
    /// Unique identifier for the transaction
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the account that owns this transaction
    /// </summary>
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Navigation property to the account
    /// </summary>
    [ForeignKey(nameof(AccountId))]
    public Account Account { get; set; } = null!;

    /// <summary>
    /// Amount of the transaction
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Timestamp when the transaction occurred
    /// </summary>
    [Required]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Foreign key to the category
    /// </summary>
    [Required]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Navigation property to the category
    /// </summary>
    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;

    /// <summary>
    /// Optional notes or description for the transaction
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
}
