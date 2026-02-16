using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Transaction entity operations
/// </summary>
public class TransactionRepository : ITransactionRepository
{
    private readonly FinanceTrackerDbContext _context;
    private readonly ILogger<TransactionRepository> _logger;
    private readonly IAccountRepository _accountRepository;

    public TransactionRepository(FinanceTrackerDbContext context, ILogger<TransactionRepository> logger, IAccountRepository accountRepository)
    {
        _context = context;
        _logger = logger;
        _accountRepository = accountRepository;
    }

    /// <summary>
    /// Creates a new transaction in the database
    /// </summary>
    /// <param name="transaction">The transaction to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created transaction with generated ID</returns>
    public async Task<Transaction> CreateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new transaction with amount: {Amount}", transaction.Amount);
        
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Update account balance
        await _accountRepository.UpdateBalanceAsync(transaction.AccountId, transaction.Amount, cancellationToken);
        
        _logger.LogInformation("Transaction created successfully with ID: {TransactionId}", transaction.Id);
        
        return transaction;
    }

    /// <summary>
    /// Updates an existing transaction in the database if it belongs to the specified user (via account ownership)
    /// </summary>
    /// <param name="transaction">The transaction to update</param>
    /// <param name="userId">The ID of the user who owns the transaction's account and category</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated transaction, or null if not found or doesn't belong to user</returns>
    public async Task<Transaction?> UpdateAsync(Transaction transaction, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating transaction with ID: {TransactionId} for user: {UserId}", transaction.Id, userId);
        
        // First verify the transaction exists and belongs to the user (via account ownership)
        var existingTransaction = await _context.Transactions
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == transaction.Id && t.Account.UserId == userId, cancellationToken);
        
        if (existingTransaction == null)
        {
            _logger.LogWarning("Transaction with ID {TransactionId} not found or does not belong to user {UserId}", transaction.Id, userId);
            return null;
        }
        
        // Calculate balance adjustment: reverse old amount and add new amount
        var oldAmount = existingTransaction.Amount;
        var balanceAdjustment = transaction.Amount - oldAmount;
        
        // Update the properties
        existingTransaction.AccountId = transaction.AccountId;
        existingTransaction.Amount = transaction.Amount;
        existingTransaction.Timestamp = transaction.Timestamp;
        existingTransaction.CategoryId = transaction.CategoryId;
        existingTransaction.Notes = transaction.Notes;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        // Update account balance with the difference
        await _accountRepository.UpdateBalanceAsync(transaction.AccountId, balanceAdjustment, cancellationToken);
        
        _logger.LogInformation("Transaction updated successfully with ID: {TransactionId} for user: {UserId}", transaction.Id, userId);
        
        return existingTransaction;
    }

    /// <summary>
    /// Gets all transactions from the database for a specific user (via account ownership)
    /// </summary>
    /// <param name="userId">The ID of the user whose transactions to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all transactions for the user</returns>
    public async Task<List<Transaction>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all transactions from database for user: {UserId}", userId);
        
        var transactions = await _context.Transactions
            .AsNoTracking()
            .Include(t => t.Account)
            .Where(t => t.Account.UserId == userId)
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} transactions from database for user: {UserId}", transactions.Count, userId);
        
        return transactions;
    }

    /// <summary>
    /// Deletes a transaction from the database if it belongs to the specified user (via account ownership)
    /// </summary>
    /// <param name="id">The ID of the transaction to delete</param>
    /// <param name="userId">The ID of the user who owns the transaction's account</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if transaction was found, belongs to user, and was deleted; false otherwise</returns>
    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting to delete transaction with ID: {TransactionId} for user: {UserId}", id, userId);
        
        var transaction = await _context.Transactions
            .Include(t => t.Account)
            .FirstOrDefaultAsync(t => t.Id == id && t.Account.UserId == userId, cancellationToken);
        
        if (transaction == null)
        {
            _logger.LogWarning("Transaction with ID {TransactionId} not found or does not belong to user {UserId}", id, userId);
            return false;
        }
        
        // Store amount and accountId before deleting
        var amount = transaction.Amount;
        var accountId = transaction.AccountId;
        
        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Update account balance by reversing the transaction amount
        await _accountRepository.UpdateBalanceAsync(accountId, -amount, cancellationToken);
        
        _logger.LogInformation("Transaction with ID {TransactionId} deleted successfully for user: {UserId}", id, userId);
        
        return true;
    }

    /// <summary>
    /// Validates that the account belongs to the specified user
    /// </summary>
    /// <param name="accountId">The ID of the account to validate</param>
    /// <param name="userId">The ID of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if account belongs to user; false otherwise</returns>
    public async Task<bool> ValidateAccountOwnershipAsync(Guid accountId, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating account ownership: AccountId={AccountId}, UserId={UserId}", accountId, userId);
        
        var accountExists = await _context.Accounts
            .AsNoTracking()
            .AnyAsync(a => a.Id == accountId && a.UserId == userId, cancellationToken);
        
        if (!accountExists)
        {
            _logger.LogWarning("Account with ID {AccountId} not found or does not belong to user {UserId}", accountId, userId);
        }
        
        return accountExists;
    }

    /// <summary>
    /// Validates that the category belongs to the specified user
    /// </summary>
    /// <param name="categoryId">The ID of the category to validate</param>
    /// <param name="userId">The ID of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if category belongs to user; false otherwise</returns>
    public async Task<bool> ValidateCategoryOwnershipAsync(Guid categoryId, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Validating category ownership: CategoryId={CategoryId}, UserId={UserId}", categoryId, userId);
        
        var categoryExists = await _context.Categories
            .AsNoTracking()
            .AnyAsync(c => c.Id == categoryId && c.UserId == userId, cancellationToken);
        
        if (!categoryExists)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found or does not belong to user {UserId}", categoryId, userId);
        }
        
        return categoryExists;
    }
}
