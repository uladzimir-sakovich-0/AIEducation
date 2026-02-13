using FinanceTracker.API.Controllers;
using FinanceTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FinanceTracker.UnitTests;

/// <summary>
/// Tests for the HealthController
/// </summary>
public class HealthControllerTests
{
    private Mock<IHealthService> GetMockHealthService()
    {
        return new Mock<IHealthService>();
    }

    [Fact]
    public async Task WhenHealthCheckIsRequested_ThenReturnsOkResult()
    {
        // Arrange
        var mockHealthService = GetMockHealthService();
        mockHealthService.Setup(s => s.CheckHealthAsync())
            .ReturnsAsync(new HealthCheckResult
            {
                IsHealthy = true,
                Timestamp = DateTime.UtcNow,
                DatabaseVersion = "PostgreSQL 16.12"
            });
        
        var controller = new HealthController(mockHealthService.Object);

        // Act
        var result = await controller.Get();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task WhenHealthCheckIsRequested_ThenReturnsHealthyStatus()
    {
        // Arrange
        var mockHealthService = GetMockHealthService();
        mockHealthService.Setup(s => s.CheckHealthAsync())
            .ReturnsAsync(new HealthCheckResult
            {
                IsHealthy = true,
                Timestamp = DateTime.UtcNow,
                DatabaseVersion = "PostgreSQL 16.12"
            });
        
        var controller = new HealthController(mockHealthService.Object);

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
        var mockHealthService = GetMockHealthService();
        var beforeCall = DateTime.UtcNow;
        
        mockHealthService.Setup(s => s.CheckHealthAsync())
            .ReturnsAsync(new HealthCheckResult
            {
                IsHealthy = true,
                Timestamp = DateTime.UtcNow,
                DatabaseVersion = "PostgreSQL 16.12"
            });
        
        var controller = new HealthController(mockHealthService.Object);

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
        var mockHealthService = GetMockHealthService();
        mockHealthService.Setup(s => s.CheckHealthAsync())
            .ReturnsAsync(new HealthCheckResult
            {
                IsHealthy = true,
                Timestamp = DateTime.UtcNow,
                DatabaseVersion = "PostgreSQL 16.12"
            });
        
        var controller = new HealthController(mockHealthService.Object);

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

    [Fact]
    public async Task WhenHealthCheckReturnsUnhealthy_ThenReturnsUnhealthyStatus()
    {
        // Arrange
        var mockHealthService = GetMockHealthService();
        mockHealthService.Setup(s => s.CheckHealthAsync())
            .ReturnsAsync(new HealthCheckResult
            {
                IsHealthy = false,
                Timestamp = DateTime.UtcNow,
                DatabaseVersion = "Unknown"
            });
        
        var controller = new HealthController(mockHealthService.Object);

        // Act
        var result = await controller.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        var response = result.Value;
        Assert.NotNull(response);
        
        // Verify response has status property with "Unhealthy" value
        var statusProperty = response.GetType().GetProperty("status");
        Assert.NotNull(statusProperty);
        Assert.Equal("Unhealthy", statusProperty.GetValue(response));
    }
}
