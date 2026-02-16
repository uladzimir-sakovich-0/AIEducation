using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.API.Controllers;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>200 OK with JWT token, or 401 Unauthorized if authentication fails</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(Infrastructure.Models.Responses.LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login request received for email: {Email}", request.Email);

        var response = await _authService.LoginAsync(request, cancellationToken);

        if (response == null)
        {
            _logger.LogWarning("Login failed for email: {Email}", request.Email);
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication Failed",
                Detail = "Invalid email or password",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        _logger.LogInformation("Login successful for email: {Email}", request.Email);
        return Ok(response);
    }
}
