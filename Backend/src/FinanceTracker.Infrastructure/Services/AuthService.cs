using FinanceTracker.Infrastructure.Data;
using FinanceTracker.Infrastructure.Models.Requests;
using FinanceTracker.Infrastructure.Models.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Service implementation for authentication operations
/// </summary>
public class AuthService : IAuthService
{
    private readonly FinanceTrackerDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        FinanceTrackerDbContext context,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user and generates a JWT token
    /// </summary>
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        // Find user by email
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found for email {Email}", request.Email);
            return null;
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed: User account is inactive for email {Email}", request.Email);
            return null;
        }

        // Verify password hash
        var passwordHash = HashPassword(request.Password);
        if (user.PasswordHash != passwordHash)
        {
            _logger.LogWarning("Login failed: Invalid password for email {Email}", request.Email);
            return null;
        }

        // Generate JWT token
        var token = GenerateJwtToken(user.Id, user.Email);
        var expirationHours = int.TryParse(_configuration["Jwt:ExpirationHours"], out var hours) ? hours : 24;
        var expiresAt = DateTime.UtcNow.AddHours(expirationHours);

        _logger.LogInformation("Login successful for email: {Email}", request.Email);

        return new LoginResponse
        {
            Token = token,
            Email = user.Email,
            ExpiresAt = expiresAt
        };
    }

    /// <summary>
    /// Generates a JWT token for the authenticated user
    /// </summary>
    private string GenerateJwtToken(Guid userId, string email)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key not configured")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expirationHours = int.TryParse(_configuration["Jwt:ExpirationHours"], out var hours) ? hours : 24;

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Hashes a password using SHA256
    /// </summary>
    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
