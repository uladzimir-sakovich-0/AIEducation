namespace FinanceTracker.Infrastructure.Models.Responses;

/// <summary>
/// Response model for successful login
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// User's email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in UTC
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}
