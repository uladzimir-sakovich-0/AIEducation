using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;

namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Service interface for authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticates a user and generates a JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Login response with JWT token, or null if authentication fails</returns>
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
