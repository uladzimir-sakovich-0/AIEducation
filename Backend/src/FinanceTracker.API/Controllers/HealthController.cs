using Microsoft.AspNetCore.Mvc;
using FinanceTracker.Infrastructure.Services;

namespace FinanceTracker.API.Controllers;

/// <summary>
/// Health check controller for monitoring application status
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IHealthService _healthService;

    public HealthController(IHealthService healthService)
    {
        _healthService = healthService;
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>200 OK if the service is running</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var healthResult = await _healthService.CheckHealthAsync();
        
        return Ok(new 
        { 
            status = healthResult.IsHealthy ? "Healthy" : "Unhealthy", 
            timestamp = healthResult.Timestamp,
            databaseVersion = healthResult.DatabaseVersion ?? "Unknown"
        });
    }
}
