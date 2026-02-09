using FinanceTracker.API.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.UnitTests;

/// <summary>
/// Tests for the HealthController
/// </summary>
public class HealthControllerTests
{
    [Fact]
    public void WhenHealthCheckIsRequested_ThenReturnsOkResult()
    {
        // Arrange
        var controller = new HealthController();

        // Act
        var result = controller.Get();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void WhenHealthCheckIsRequested_ThenReturnsHealthyStatus()
    {
        // Arrange
        var controller = new HealthController();

        // Act
        var result = controller.Get() as OkObjectResult;

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
    public void WhenHealthCheckIsRequested_ThenReturnsTimestamp()
    {
        // Arrange
        var controller = new HealthController();
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = controller.Get() as OkObjectResult;
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
}
