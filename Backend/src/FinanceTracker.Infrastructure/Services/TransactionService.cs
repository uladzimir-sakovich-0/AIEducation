using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;
using FinanceTracker.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Service implementation for transaction business logic
/// </summary>
public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ITransactionRepository repository, ILogger<TransactionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new transaction
    /// </summary>
    /// <param name="request">Transaction creation request</param>
    /// <param name="userId">The ID of the user creating the transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The ID of the created transaction, or null if account or category doesn't belong to user</returns>
    public async Task<Guid?> CreateTransactionAsync(TransactionCreateRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating transaction with amount: {Amount} for user: {UserId}", request.Amount, userId);
        
        // Validate that the account belongs to the user
        var accountBelongsToUser = await _repository.ValidateAccountOwnershipAsync(request.AccountId, userId, cancellationToken);
        if (!accountBelongsToUser)
        {
            _logger.LogWarning("Account with ID {AccountId} does not belong to user {UserId}", request.AccountId, userId);
            return null;
        }
        
        // Validate that the category belongs to the user
        var categoryBelongsToUser = await _repository.ValidateCategoryOwnershipAsync(request.CategoryId, userId, cancellationToken);
        if (!categoryBelongsToUser)
        {
            _logger.LogWarning("Category with ID {CategoryId} does not belong to user {UserId}", request.CategoryId, userId);
            return null;
        }
        
        var transaction = MapToEntity(request);

        var createdTransaction = await _repository.CreateAsync(transaction, cancellationToken);
        
        _logger.LogInformation("Transaction created with ID: {TransactionId} for user: {UserId}", createdTransaction.Id, userId);
        
        return createdTransaction.Id;
    }

    /// <summary>
    /// Updates an existing transaction
    /// </summary>
    /// <param name="request">Transaction update request</param>
    /// <param name="userId">The ID of the user updating the transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if transaction was found, belongs to user, and was updated; false otherwise</returns>
    public async Task<bool> UpdateTransactionAsync(TransactionUpdateRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating transaction with ID: {TransactionId} for user: {UserId}", request.Id, userId);
        
        // Validate that the account belongs to the user
        var accountBelongsToUser = await _repository.ValidateAccountOwnershipAsync(request.AccountId, userId, cancellationToken);
        if (!accountBelongsToUser)
        {
            _logger.LogWarning("Account with ID {AccountId} does not belong to user {UserId}", request.AccountId, userId);
            return false;
        }
        
        // Validate that the category belongs to the user
        var categoryBelongsToUser = await _repository.ValidateCategoryOwnershipAsync(request.CategoryId, userId, cancellationToken);
        if (!categoryBelongsToUser)
        {
            _logger.LogWarning("Category with ID {CategoryId} does not belong to user {UserId}", request.CategoryId, userId);
            return false;
        }
        
        var transaction = MapToEntity(request);

        var updatedTransaction = await _repository.UpdateAsync(transaction, userId, cancellationToken);
        
        if (updatedTransaction == null)
        {
            _logger.LogWarning("Transaction with ID {TransactionId} not found or does not belong to user {UserId}", request.Id, userId);
            return false;
        }
        
        _logger.LogInformation("Transaction updated with ID: {TransactionId} for user: {UserId}", transaction.Id, userId);
        return true;
    }

    /// <summary>
    /// Gets all transactions for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose transactions to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all transactions for the user as DTOs</returns>
    public async Task<List<TransactionDto>> GetAllTransactionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all transactions for user: {UserId}", userId);
        
        var transactions = await _repository.GetAllAsync(userId, cancellationToken);
        
        var transactionDtos = transactions.Select(MapToDto).ToList();
        
        _logger.LogInformation("{Count} transactions retrieved for user: {UserId}", transactionDtos.Count, userId);
        
        return transactionDtos;
    }

    /// <summary>
    /// Deletes a transaction if it belongs to the specified user
    /// </summary>
    /// <param name="id">The ID of the transaction to delete</param>
    /// <param name="userId">The ID of the user who owns the transaction</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if transaction was found, belongs to user, and was deleted; false otherwise</returns>
    public async Task<bool> DeleteTransactionAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting transaction with ID: {TransactionId} for user: {UserId}", id, userId);
        
        var deleted = await _repository.DeleteAsync(id, userId, cancellationToken);
        
        if (deleted)
        {
            _logger.LogInformation("Transaction deleted successfully with ID: {TransactionId} for user: {UserId}", id, userId);
        }
        else
        {
            _logger.LogWarning("Transaction with ID {TransactionId} not found or does not belong to user {UserId}", id, userId);
        }
        
        return deleted;
    }

    /// <summary>
    /// Maps Transaction entity to TransactionDto
    /// </summary>
    /// <param name="transaction">The transaction entity</param>
    /// <returns>TransactionDto</returns>
    private TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            AccountId = transaction.AccountId,
            Amount = transaction.Amount,
            Timestamp = transaction.Timestamp,
            CategoryId = transaction.CategoryId,
            Notes = transaction.Notes
        };
    }

    /// <summary>
    /// Maps TransactionCreateRequest to Transaction entity
    /// </summary>
    /// <param name="request">The request model</param>
    /// <returns>Transaction entity</returns>
    private Transaction MapToEntity(TransactionCreateRequest request)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = request.AccountId,
            Amount = request.Amount,
            Timestamp = request.Timestamp,
            CategoryId = request.CategoryId,
            Notes = request.Notes
        };
    }

    /// <summary>
    /// Maps TransactionUpdateRequest to Transaction entity
    /// </summary>
    /// <param name="request">The request model</param>
    /// <returns>Transaction entity</returns>
    private Transaction MapToEntity(TransactionUpdateRequest request)
    {
        return new Transaction
        {
            Id = request.Id,
            AccountId = request.AccountId,
            Amount = request.Amount,
            Timestamp = request.Timestamp,
            CategoryId = request.CategoryId,
            Notes = request.Notes
        };
    }
}
