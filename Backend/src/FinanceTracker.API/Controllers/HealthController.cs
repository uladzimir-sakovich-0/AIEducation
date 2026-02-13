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
            // Query PostgreSQL version - this will work for PostgreSQL but not InMemory database
            version = await _context.Database.SqlQueryRaw<string>("SELECT version()").FirstOrDefaultAsync();
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
