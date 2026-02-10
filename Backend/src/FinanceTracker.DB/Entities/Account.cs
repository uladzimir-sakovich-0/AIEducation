using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceTracker.DB.Entities;

/// <summary>
/// Represents a financial account (e.g., checking, savings, credit card)
/// </summary>
public class Account
{
    /// <summary>
    /// Unique identifier for the account
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the user who owns this account
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    /// <summary>
    /// Name of the account (e.g., "Main Checking", "Savings")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of account (e.g., "Checking", "Savings", "Credit Card")
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string AccountType { get; set; } = string.Empty;

    /// <summary>
    /// Current balance of the account
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Balance { get; set; }

    /// <summary>
    /// Timestamp when the account was created
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Navigation property for transactions associated with this account
    /// </summary>
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
