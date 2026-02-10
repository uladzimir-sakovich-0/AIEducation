using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.DB.Entities;

/// <summary>
/// Represents a user account in the finance tracker system
/// </summary>
public class Account
{
    /// <summary>
    /// Unique identifier for the account
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Email address of the account owner
    /// </summary>
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password for account security
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

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
