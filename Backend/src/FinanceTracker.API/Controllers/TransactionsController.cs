using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;
using FinanceTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

/// <summary>
/// Controller for transaction management operations
/// </summary>
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : BaseApiController
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new transaction
    /// </summary>
    /// <param name="request">Transaction creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>201 Created with the transaction ID, or 400 Bad Request for validation errors or if account/category doesn't belong to user</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTransaction(
        [FromBody] TransactionCreateRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to create transaction with amount: {Amount} for user: {UserId}", request.Amount, userId);
        
        var transactionId = await _transactionService.CreateTransactionAsync(request, userId, cancellationToken);
        
        if (transactionId == null)
        {
            _logger.LogWarning("Failed to create transaction for user {UserId} - account or category not found or unauthorized", userId);
            return BadRequest(new ProblemDetails
            {
                Title = "Transaction Creation Failed",
                Detail = "The specified account or category does not exist or you do not have permission to use it.",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        _logger.LogInformation("Transaction created successfully with ID: {TransactionId} for user: {UserId}", transactionId, userId);
        
        return Created(string.Empty, transactionId);
    }

    /// <summary>
    /// Updates an existing transaction
    /// </summary>
    /// <param name="request">Transaction update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>200 OK on successful update, or 400 Bad Request if transaction not found or doesn't belong to user or account/category unauthorized</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTransaction(
        [FromBody] TransactionUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to update transaction with ID: {TransactionId} for user: {UserId}", request.Id, userId);
        
        var updated = await _transactionService.UpdateTransactionAsync(request, userId, cancellationToken);
        
        if (!updated)
        {
            _logger.LogWarning("Transaction with ID {TransactionId} not found or does not belong to user {UserId}, or account/category unauthorized", request.Id, userId);
            return BadRequest(new ProblemDetails
            {
                Title = "Transaction Not Found",
                Detail = $"Transaction with ID {request.Id} does not exist or you do not have permission to update it, or the specified account/category is unauthorized.",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        _logger.LogInformation("Transaction updated successfully with ID: {TransactionId} for user: {UserId}", request.Id, userId);
        
        return Ok();
    }

    /// <summary>
    /// Gets all transactions
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>200 OK with list of transactions</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTransactions(CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to get all transactions for user: {UserId}", userId);
        
        var transactions = await _transactionService.GetAllTransactionsAsync(userId, cancellationToken);
        
        _logger.LogInformation("Returning {Count} transactions for user: {UserId}", transactions.Count, userId);
        
        return Ok(transactions);
    }

    /// <summary>
    /// Deletes a transaction
    /// </summary>
    /// <param name="id">The ID of the transaction to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>200 OK when deleted, or 400 Bad Request if not found</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteTransaction(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to delete transaction with ID: {TransactionId} for user: {UserId}", id, userId);
        
        var deleted = await _transactionService.DeleteTransactionAsync(id, userId, cancellationToken);
        
        if (!deleted)
        {
            _logger.LogWarning("Transaction with ID {TransactionId} not found or does not belong to user {UserId}", id, userId);
            return BadRequest(new ProblemDetails
            {
                Title = "Transaction Not Found",
                Detail = $"Transaction with ID {id} does not exist or you do not have permission to delete it.",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        _logger.LogInformation("Transaction deleted successfully with ID: {TransactionId} for user: {UserId}", id, userId);
        
        return Ok();
    }
}
