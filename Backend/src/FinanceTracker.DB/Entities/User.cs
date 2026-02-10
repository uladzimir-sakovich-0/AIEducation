using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.DB.Entities;

/// <summary>
/// Represents a user in the finance tracker system
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Email address of the user
    /// </summary>
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password for user authentication
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp when the user was created
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Navigation property for financial accounts owned by this user
    /// </summary>
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
