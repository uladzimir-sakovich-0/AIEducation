using Microsoft.AspNetCore.Mvc;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.API.Controllers;

/// <summary>
/// Health check controller for monitoring application status
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly FinanceTrackerDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(FinanceTrackerDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>200 OK if the service is running</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        string? version = null;
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
                    var result = await command.ExecuteScalarAsync();
                    version = result?.ToString();
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
            version = "Unknown";
        }
        
        return Ok(new 
        { 
            status = "Healthy", 
            timestamp = DateTime.UtcNow,
            databaseVersion = version ?? "Unknown"
        });
    }
}
