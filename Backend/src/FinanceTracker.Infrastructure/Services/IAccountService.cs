using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;

namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Service interface for account business logic
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Creates a new account
    /// </summary>
    /// <param name="request">Account creation request</param>
    /// <param name="userId">The ID of the user creating the account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created account</returns>
    Task<Guid> CreateAccountAsync(AccountCreateRequest request, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing account
    /// </summary>
    /// <param name="request">Account update request</param>
    /// <param name="userId">The ID of the user updating the account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if account was found, belongs to user, and was updated; false otherwise</returns>
    Task<bool> UpdateAccountAsync(AccountUpdateRequest request, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all accounts for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose accounts to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all accounts for the user as DTOs</returns>
    Task<List<AccountDto>> GetAllAccountsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an account if it belongs to the specified user
    /// </summary>
    /// <param name="id">The ID of the account to delete</param>
    /// <param name="userId">The ID of the user who owns the account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if account was found, belongs to user, and was deleted; false otherwise</returns>
    Task<bool> DeleteAccountAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
