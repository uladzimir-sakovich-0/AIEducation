using FinanceTracker.DB.Entities;

namespace FinanceTracker.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Transaction entity operations
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Creates a new transaction in the database
    /// </summary>
    /// <param name="transaction">The transaction to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created transaction with generated ID</returns>
    Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing transaction in the database if it belongs to the specified user (via account ownership)
    /// </summary>
    /// <param name="transaction">The transaction to update</param>
    /// <param name="userId">The ID of the user who owns the transaction's account and category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated transaction, or null if not found or doesn't belong to user</returns>
    Task<Transaction?> UpdateAsync(Transaction transaction, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all transactions from the database for a specific user (via account ownership)
    /// </summary>
    /// <param name="userId">The ID of the user whose transactions to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all transactions for the user</returns>
    Task<List<Transaction>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a transaction from the database if it belongs to the specified user (via account ownership)
    /// </summary>
    /// <param name="id">The ID of the transaction to delete</param>
    /// <param name="userId">The ID of the user who owns the transaction's account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if transaction was found, belongs to user, and was deleted; false otherwise</returns>
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that the account belongs to the specified user
    /// </summary>
    /// <param name="accountId">The ID of the account to validate</param>
    /// <param name="userId">The ID of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if account belongs to user; false otherwise</returns>
    Task<bool> ValidateAccountOwnershipAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that the category belongs to the specified user
    /// </summary>
    /// <param name="categoryId">The ID of the category to validate</param>
    /// <param name="userId">The ID of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if category belongs to user; false otherwise</returns>
    Task<bool> ValidateCategoryOwnershipAsync(Guid categoryId, Guid userId, CancellationToken cancellationToken = default);
}
