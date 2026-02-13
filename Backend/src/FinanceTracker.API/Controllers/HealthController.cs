using Microsoft.AspNetCore.Mvc;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FinanceTracker.API.Controllers;

/// <summary>
/// Health check controller for monitoring application status
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly FinanceTrackerDbContext _context;

    public HealthController(FinanceTrackerDbContext context)
    {
        _context = context;
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
            await connection.OpenAsync();
            
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT version()";
                var result = await command.ExecuteScalarAsync();
                version = result?.ToString();
            }
            
            await connection.CloseAsync();
        }
        catch
        {
            // If database query fails (e.g., InMemory database in tests), use "Unknown"
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
