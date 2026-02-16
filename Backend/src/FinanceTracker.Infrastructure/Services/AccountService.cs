using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;
using FinanceTracker.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Service implementation for account business logic
/// </summary>
public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    private readonly ILogger<AccountService> _logger;

    public AccountService(IAccountRepository repository, ILogger<AccountService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new account
    /// </summary>
    /// <param name="request">Account creation request</param>
    /// <param name="userId">The ID of the user creating the account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created account</returns>
    public async Task<Guid> CreateAccountAsync(AccountCreateRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating account with name: {AccountName} for user: {UserId}", request.Name, userId);
        
        var account = MapToEntity(request, userId);

        var createdAccount = await _repository.CreateAsync(account, cancellationToken);
        
        _logger.LogInformation("Account created with ID: {AccountId} for user: {UserId}", createdAccount.Id, userId);
        
        return createdAccount.Id;
    }

    /// <summary>
    /// Updates an existing account
    /// </summary>
    /// <param name="request">Account update request</param>
    /// <param name="userId">The ID of the user updating the account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if account was found, belongs to user, and was updated; false otherwise</returns>
    public async Task<bool> UpdateAccountAsync(AccountUpdateRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating account with ID: {AccountId} for user: {UserId}", request.Id, userId);
        
        var account = MapToEntity(request, userId);

        var updatedAccount = await _repository.UpdateAsync(account, userId, cancellationToken);
        
        if (updatedAccount == null)
        {
            _logger.LogWarning("Account with ID {AccountId} not found or does not belong to user {UserId}", request.Id, userId);
            return false;
        }
        
        _logger.LogInformation("Account updated with ID: {AccountId} for user: {UserId}", account.Id, userId);
        return true;
    }

    /// <summary>
    /// Gets all accounts for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose accounts to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all accounts for the user as DTOs</returns>
    public async Task<List<AccountDto>> GetAllAccountsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all accounts for user: {UserId}", userId);
        
        var accounts = await _repository.GetAllAsync(userId, cancellationToken);
        
        var accountDtos = accounts.Select(MapToDto).ToList();
        
        _logger.LogInformation("{Count} accounts retrieved for user: {UserId}", accountDtos.Count, userId);
        
        return accountDtos;
    }

    /// <summary>
    /// Deletes an account if it belongs to the specified user
    /// </summary>
    /// <param name="id">The ID of the account to delete</param>
    /// <param name="userId">The ID of the user who owns the account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if account was found, belongs to user, and was deleted; false otherwise</returns>
    public async Task<bool> DeleteAccountAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting account with ID: {AccountId} for user: {UserId}", id, userId);
        
        var deleted = await _repository.DeleteAsync(id, userId, cancellationToken);
        
        if (deleted)
        {
            _logger.LogInformation("Account deleted successfully with ID: {AccountId} for user: {UserId}", id, userId);
        }
        else
        {
            _logger.LogWarning("Account with ID {AccountId} not found or does not belong to user {UserId}", id, userId);
        }
        
        return deleted;
    }

    /// <summary>
    /// Maps Account entity to AccountDto
    /// </summary>
    /// <param name="account">The account entity</param>
    /// <returns>AccountDto</returns>
    private AccountDto MapToDto(Account account)
    {
        return new AccountDto
        {
            Id = account.Id,
            Name = account.Name,
            AccountType = account.AccountType,
            Balance = account.Balance,
            CreatedAt = account.CreatedAt
        };
    }

    /// <summary>
    /// Maps AccountCreateRequest to Account entity
    /// </summary>
    /// <param name="request">The request model</param>
    /// <param name="userId">The ID of the user creating the account</param>
    /// <returns>Account entity</returns>
    private Account MapToEntity(AccountCreateRequest request, Guid userId)
    {
        return new Account
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            AccountType = request.AccountType,
            Balance = request.Balance,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Maps AccountUpdateRequest to Account entity
    /// </summary>
    /// <param name="request">The request model</param>
    /// <param name="userId">The ID of the user updating the account</param>
    /// <returns>Account entity</returns>
    private Account MapToEntity(AccountUpdateRequest request, Guid userId)
    {
        return new Account
        {
            Id = request.Id,
            Name = request.Name,
            AccountType = request.AccountType,
            Balance = request.Balance,
            UserId = userId
        };
    }
}
