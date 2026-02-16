namespace FinanceTracker.Infrastructure.Models.Requests;

/// <summary>
/// Request model for updating an existing account
/// </summary>
public class AccountUpdateRequest
{
    /// <summary>
    /// ID of the account to update
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
    /// Balance of the account
    /// </summary>
    public decimal Balance { get; set; }
}
