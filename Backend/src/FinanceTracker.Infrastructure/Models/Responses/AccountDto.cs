namespace FinanceTracker.Infrastructure.Models.Responses;

/// <summary>
/// Data transfer object for Account
/// </summary>
public class AccountDto
{
    /// <summary>
    /// Unique identifier for the account
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the account
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of account (e.g., "Cash", "Bank")
    /// </summary>
    public string AccountType { get; set; } = string.Empty;

    /// <summary>
    /// Current balance of the account
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Timestamp when the account was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
