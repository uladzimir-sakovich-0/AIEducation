namespace FinanceTracker.Infrastructure.Models.Requests;

/// <summary>
/// Request model for updating an existing transaction
/// </summary>
public class TransactionUpdateRequest
{
    /// <summary>
    /// ID of the transaction to update
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the account that owns this transaction
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Amount of the transaction (positive for income, negative for expense)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Timestamp when the transaction occurred
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Foreign key to the category
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// Optional notes or description for the transaction
    /// </summary>
    public string? Notes { get; set; }
}
