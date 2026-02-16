using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;
using FinanceTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

/// <summary>
/// Controller for account management operations
/// </summary>
[Route("api/[controller]")]
[Authorize]
public class AccountsController : BaseApiController
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new account
    /// </summary>
    /// <param name="request">Account creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>201 Created with the account ID, or 400 Bad Request for validation errors</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAccount(
        [FromBody] AccountCreateRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to create account with name: {AccountName} for user: {UserId}", request.Name, userId);
        
        var accountId = await _accountService.CreateAccountAsync(request, userId, cancellationToken);
        
        _logger.LogInformation("Account created successfully with ID: {AccountId} for user: {UserId}", accountId, userId);
        
        return Created(string.Empty, accountId);
    }

    /// <summary>
    /// Updates an existing account
    /// </summary>
    /// <param name="request">Account update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>200 OK on successful update, or 400 Bad Request if account not found or doesn't belong to user</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAccount(
        [FromBody] AccountUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to update account with ID: {AccountId} for user: {UserId}", request.Id, userId);
        
        var updated = await _accountService.UpdateAccountAsync(request, userId, cancellationToken);
        
        if (!updated)
        {
            _logger.LogWarning("Account with ID {AccountId} not found or does not belong to user {UserId}", request.Id, userId);
            return BadRequest(new ProblemDetails
            {
                Title = "Account Not Found",
                Detail = $"Account with ID {request.Id} does not exist or you do not have permission to update it.",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        _logger.LogInformation("Account updated successfully with ID: {AccountId} for user: {UserId}", request.Id, userId);
        
        return Ok();
    }

    /// <summary>
    /// Gets all accounts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>200 OK with list of accounts</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<AccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAccounts(CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to get all accounts for user: {UserId}", userId);
        
        var accounts = await _accountService.GetAllAccountsAsync(userId, cancellationToken);
        
        _logger.LogInformation("Returning {Count} accounts for user: {UserId}", accounts.Count, userId);
        
        return Ok(accounts);
    }

    /// <summary>
    /// Deletes an account
    /// </summary>
    /// <param name="id">The ID of the account to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>200 OK when deleted, or 400 Bad Request if not found</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteAccount(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserIdFromClaims();
        
        _logger.LogInformation("Received request to delete account with ID: {AccountId} for user: {UserId}", id, userId);
        
        var deleted = await _accountService.DeleteAccountAsync(id, userId, cancellationToken);
        
        if (!deleted)
        {
            _logger.LogWarning("Account with ID {AccountId} not found or does not belong to user {UserId}", id, userId);
            return BadRequest(new ProblemDetails
            {
                Title = "Account Not Found",
                Detail = $"Account with ID {id} does not exist or you do not have permission to delete it.",
                Status = StatusCodes.Status400BadRequest
            });
        }
        
        _logger.LogInformation("Account deleted successfully with ID: {AccountId} for user: {UserId}", id, userId);
        
        return Ok();
    }
}
