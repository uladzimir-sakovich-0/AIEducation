using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Service for performing health checks
/// </summary>
public class HealthService : IHealthService
{
    private readonly FinanceTrackerDbContext _context;
    private readonly ILogger<HealthService> _logger;

    public HealthService(FinanceTrackerDbContext context, ILogger<HealthService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Performs a health check including database connectivity
    /// </summary>
    /// <returns>Health check result with status and database version</returns>
    public async Task<HealthCheckResult> CheckHealthAsync()
    {
        var result = new HealthCheckResult
        {
            IsHealthy = true,
            Timestamp = DateTime.UtcNow
        };

        try
        {
            // Query PostgreSQL version using raw connection
            var connection = _context.Database.GetDbConnection();
            
            try
            {
                await connection.OpenAsync();
                
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT version()";
                    var version = await command.ExecuteScalarAsync();
                    result.DatabaseVersion = version?.ToString();
                }
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception for debugging purposes
            _logger.LogWarning(ex, "Failed to query database version. Using 'Unknown' as fallback.");
            result.DatabaseVersion = "Unknown";
        }

        return result;
    }
}
