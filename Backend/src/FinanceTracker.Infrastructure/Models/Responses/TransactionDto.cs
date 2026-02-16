namespace FinanceTracker.Infrastructure.Models.Responses;

/// <summary>
/// Data transfer object for Transaction
/// </summary>
public class TransactionDto
{
    /// <summary>
    /// Unique identifier for the transaction
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to the account that owns this transaction
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Amount of the transaction
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

    /// <summary>
    /// Type of transaction operation based on amount (Expense if negative, Income if positive)
    /// </summary>
    public string OperationType => Amount < 0 ? "Expense" : "Income";
}
