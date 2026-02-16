using FinanceTracker.DB.Entities;

namespace FinanceTracker.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Account entity operations
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Creates a new account in the database
    /// </summary>
    /// <param name="account">The account to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created account with generated ID</returns>
    Task<Account> CreateAsync(Account account, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing account in the database if it belongs to the specified user
    /// </summary>
    /// <param name="account">The account to update</param>
    /// <param name="userId">The ID of the user who owns the account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated account, or null if not found or doesn't belong to user</returns>
    Task<Account?> UpdateAsync(Account account, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all accounts from the database for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose accounts to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all accounts for the user</returns>
    Task<List<Account>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an account from the database if it belongs to the specified user
    /// </summary>
    /// <param name="id">The ID of the account to delete</param>
    /// <param name="userId">The ID of the user who owns the account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if account was found, belongs to user, and was deleted; false otherwise</returns>
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
