using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;

namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Service interface for transaction business logic
/// </summary>
public interface ITransactionService
{
    /// <summary>
    /// Creates a new transaction
    /// </summary>
    /// <param name="request">Transaction creation request</param>
    /// <param name="userId">The ID of the user creating the transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created transaction, or null if account or category doesn't belong to user</returns>
    Task<Guid?> CreateTransactionAsync(TransactionCreateRequest request, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing transaction
    /// </summary>
    /// <param name="request">Transaction update request</param>
    /// <param name="userId">The ID of the user updating the transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if transaction was found, belongs to user, and was updated; false otherwise</returns>
    Task<bool> UpdateTransactionAsync(TransactionUpdateRequest request, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all transactions for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose transactions to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all transactions for the user as DTOs</returns>
    Task<List<TransactionDto>> GetAllTransactionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a transaction if it belongs to the specified user
    /// </summary>
    /// <param name="id">The ID of the transaction to delete</param>
    /// <param name="userId">The ID of the user who owns the transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if transaction was found, belongs to user, and was deleted; false otherwise</returns>
    Task<bool> DeleteTransactionAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
