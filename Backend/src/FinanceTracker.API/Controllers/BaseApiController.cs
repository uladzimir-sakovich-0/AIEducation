using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTracker.API.Controllers;

/// <summary>
/// Base controller providing common functionality for API controllers
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Extracts the user ID from JWT claims
    /// </summary>
    /// <returns>The user ID as a Guid</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user ID claim is invalid or missing</exception>
    protected Guid GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID in token");
        }
        
        return userId;
    }
}
