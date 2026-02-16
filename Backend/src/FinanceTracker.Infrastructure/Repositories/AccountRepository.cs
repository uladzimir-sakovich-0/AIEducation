using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Account entity operations
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly FinanceTrackerDbContext _context;
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(FinanceTrackerDbContext context, ILogger<AccountRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new account in the database
    /// </summary>
    /// <param name="account">The account to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created account with generated ID</returns>
    public async Task<Account> CreateAsync(Account account, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new account with name: {AccountName}", account.Name);
        
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Account created successfully with ID: {AccountId}", account.Id);
        
        return account;
    }

    /// <summary>
    /// Updates an existing account in the database if it belongs to the specified user
    /// </summary>
    /// <param name="account">The account to update</param>
    /// <param name="userId">The ID of the user who owns the account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated account, or null if not found or doesn't belong to user</returns>
    public async Task<Account?> UpdateAsync(Account account, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating account with ID: {AccountId} for user: {UserId}", account.Id, userId);
        
        // First verify the account exists and belongs to the user
        var existingAccount = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == account.Id && a.UserId == userId, cancellationToken);
        
        if (existingAccount == null)
        {
            _logger.LogWarning("Account with ID {AccountId} not found or does not belong to user {UserId}", account.Id, userId);
            return null;
        }
        
        // Update only the allowed properties
        existingAccount.Name = account.Name;
        existingAccount.AccountType = account.AccountType;
        existingAccount.Balance = account.Balance;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Account updated successfully with ID: {AccountId} for user: {UserId}", account.Id, userId);
        
        return existingAccount;
    }

    /// <summary>
    /// Gets all accounts from the database for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose accounts to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all accounts for the user</returns>
    public async Task<List<Account>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all accounts from database for user: {UserId}", userId);
        
        var accounts = await _context.Accounts
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} accounts from database for user: {UserId}", accounts.Count, userId);
        
        return accounts;
    }

    /// <summary>
    /// Deletes an account from the database if it belongs to the specified user
    /// </summary>
    /// <param name="id">The ID of the account to delete</param>
    /// <param name="userId">The ID of the user who owns the account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if account was found, belongs to user, and was deleted; false otherwise</returns>
    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete account with ID: {AccountId} for user: {UserId}", id, userId);
        
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, cancellationToken);
        
        if (account == null)
        {
            _logger.LogWarning("Account with ID {AccountId} not found or does not belong to user {UserId}", id, userId);
            return false;
        }
        
        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Account with ID {AccountId} deleted successfully for user: {UserId}", id, userId);
        
        return true;
    }
}
