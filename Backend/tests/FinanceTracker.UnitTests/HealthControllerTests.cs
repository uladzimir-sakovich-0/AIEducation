using FinanceTracker.API.Controllers;
using FinanceTracker.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.UnitTests;

/// <summary>
/// Tests for the HealthController
/// </summary>
public class HealthControllerTests
{
    private FinanceTrackerDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<FinanceTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new FinanceTrackerDbContext(options);
    }

    [Fact]
    public async Task WhenHealthCheckIsRequested_ThenReturnsOkResult()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new HealthController(context);

        // Act
        var result = await controller.Get();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task WhenHealthCheckIsRequested_ThenReturnsHealthyStatus()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new HealthController(context);

        // Act
        var result = await controller.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var response = result.Value;
        Assert.NotNull(response);
        
        // Verify response has status property with "Healthy" value
        var statusProperty = response.GetType().GetProperty("status");
        Assert.NotNull(statusProperty);
        Assert.Equal("Healthy", statusProperty.GetValue(response));
    }

    [Fact]
    public async Task WhenHealthCheckIsRequested_ThenReturnsTimestamp()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new HealthController(context);
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = await controller.Get() as OkObjectResult;
        var afterCall = DateTime.UtcNow;

        // Assert
        Assert.NotNull(result);
        var response = result.Value;
        Assert.NotNull(response);
        
        // Verify response has timestamp property
        var timestampProperty = response.GetType().GetProperty("timestamp");
        Assert.NotNull(timestampProperty);
        
        var timestampValue = timestampProperty.GetValue(response);
        Assert.NotNull(timestampValue);
        var timestamp = (DateTime)timestampValue;
        Assert.True(timestamp >= beforeCall && timestamp <= afterCall, 
            "Timestamp should be between the time before and after the call");
    }

    [Fact]
    public async Task WhenHealthCheckIsRequested_ThenReturnsDatabaseVersion()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var controller = new HealthController(context);

        // Act
        var result = await controller.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var response = result.Value;
        Assert.NotNull(response);
        
        // Verify response has databaseVersion property
        var databaseVersionProperty = response.GetType().GetProperty("databaseVersion");
        Assert.NotNull(databaseVersionProperty);
        
        var databaseVersion = databaseVersionProperty.GetValue(response);
        Assert.NotNull(databaseVersion);
    }
}
