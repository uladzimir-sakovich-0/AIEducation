namespace FinanceTracker.Infrastructure.Models.Requests;

/// <summary>
/// Request model for creating a new account
/// </summary>
public class AccountCreateRequest
{
    /// <summary>
    /// Name of the account
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of account (e.g., "Cash", "Bank")
    /// </summary>
    public string AccountType { get; set; } = string.Empty;

    /// <summary>
    /// Initial balance of the account
    /// </summary>
    public decimal Balance { get; set; }
}
