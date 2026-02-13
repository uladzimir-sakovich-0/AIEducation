using FinanceTracker.Infrastructure.Data;
using FinanceTracker.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTracker.UnitTests.Services;

/// <summary>
/// Tests for the HealthService
/// </summary>
public class HealthServiceTests
{
    private FinanceTrackerDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<FinanceTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new FinanceTrackerDbContext(options);
    }

    private ILogger<HealthService> GetMockLogger()
    {
        return new Mock<ILogger<HealthService>>().Object;
    }

    [Fact]
    public async Task WhenCheckHealthAsync_WithInMemoryDatabase_ThenReturnsUnhealthyResult()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var service = new HealthService(context, logger);

        // Act
        var result = await service.CheckHealthAsync();

        // Assert
        Assert.NotNull(result);
        // InMemory database doesn't support raw SQL queries, so health check will fail
        Assert.False(result.IsHealthy);
    }

    [Fact]
    public async Task WhenCheckHealthAsync_ThenReturnsTimestamp()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var service = new HealthService(context, logger);
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = await service.CheckHealthAsync();
        var afterCall = DateTime.UtcNow;

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Timestamp >= beforeCall && result.Timestamp <= afterCall,
            "Timestamp should be between the time before and after the call");
    }

    [Fact]
    public async Task WhenCheckHealthAsync_ThenReturnsDatabaseVersion()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var service = new HealthService(context, logger);

        // Act
        var result = await service.CheckHealthAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.DatabaseVersion);
    }

    [Fact]
    public async Task WhenDatabaseQueryFails_ThenReturnsUnknownVersionAndIsUnhealthy()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var service = new HealthService(context, logger);

        // Act
        var result = await service.CheckHealthAsync();

        // Assert
        Assert.NotNull(result);
        // InMemory database will fail the version query, so it should return "Unknown"
        Assert.Equal("Unknown", result.DatabaseVersion);
        // And IsHealthy should be false due to the exception
        Assert.False(result.IsHealthy);
    }
}
