using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Data;
using FinanceTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTracker.UnitTests.Repositories;

/// <summary>
/// Tests for CategoryRepository
/// </summary>
public class CategoryRepositoryTests
{
    private FinanceTrackerDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<FinanceTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new FinanceTrackerDbContext(options);
    }

    private Mock<ILogger<CategoryRepository>> GetMockLogger()
    {
        return new Mock<ILogger<CategoryRepository>>();
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenCategoryIsSavedWithId()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Food"
        };

        // Act
        var result = await repository.CreateAsync(category);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal("Food", result.Name);
        
        // Verify it was actually saved to the database
        var savedCategory = await context.Categories.FindAsync(category.Id);
        Assert.NotNull(savedCategory);
        Assert.Equal("Food", savedCategory.Name);
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenCategoryIsAddedToDatabase()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Transportation"
        };

        // Act
        await repository.CreateAsync(category);

        // Assert
        var count = await context.Categories.CountAsync();
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task WhenCreatingMultipleCategories_ThenAllAreSaved()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var category1 = new Category { Id = Guid.NewGuid(), Name = "Food" };
        var category2 = new Category { Id = Guid.NewGuid(), Name = "Housing" };

        // Act
        await repository.CreateAsync(category1);
        await repository.CreateAsync(category2);

        // Assert
        var count = await context.Categories.CountAsync();
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task WhenCreatingCategory_ThenLogsInformation()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Entertainment"
        };

        // Act
        await repository.CreateAsync(category);

        // Assert
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating new category")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
