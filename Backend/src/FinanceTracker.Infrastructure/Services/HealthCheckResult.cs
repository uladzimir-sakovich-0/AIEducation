namespace FinanceTracker.Infrastructure.Services;

/// <summary>
/// Result of a health check operation
/// </summary>
public class HealthCheckResult
{
    /// <summary>
    /// Gets or sets whether the service is healthy
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// Gets or sets the database version information
    /// </summary>
    public string? DatabaseVersion { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the health check
    /// </summary>
    public DateTime Timestamp { get; set; }
}
