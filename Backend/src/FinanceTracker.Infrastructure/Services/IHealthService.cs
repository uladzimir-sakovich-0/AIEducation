namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Service for performing health checks
/// </summary>
public interface IHealthService
{
    /// <summary>
    /// Performs a health check including database connectivity
    /// </summary>
    /// <returns>Health check result with status and database version</returns>
    Task<HealthCheckResult> CheckHealthAsync();
}
