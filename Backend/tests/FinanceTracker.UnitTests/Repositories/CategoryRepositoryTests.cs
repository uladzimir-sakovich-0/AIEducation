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

    [Fact]
    public async Task WhenUpdatingCategory_ThenCategoryIsUpdatedInDatabase()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Food"
        };
        
        await repository.CreateAsync(category);

        // Act
        var updatedCategory = new Category
        {
            Id = categoryId,
            Name = "Updated Food"
        };
        var result = await repository.UpdateAsync(updatedCategory);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
        Assert.Equal("Updated Food", result.Name);
        
        // Verify it was actually updated in the database
        var savedCategory = await context.Categories.FindAsync(categoryId);
        Assert.NotNull(savedCategory);
        Assert.Equal("Updated Food", savedCategory.Name);
    }

    [Fact]
    public async Task WhenUpdatingCategory_ThenOnlyTargetCategoryIsModified()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();
        
        await repository.CreateAsync(new Category { Id = category1Id, Name = "Category1" });
        await repository.CreateAsync(new Category { Id = category2Id, Name = "Category2" });

        // Act
        var updatedCategory = new Category
        {
            Id = category1Id,
            Name = "Updated Category1"
        };
        await repository.UpdateAsync(updatedCategory);

        // Assert
        var category1 = await context.Categories.FindAsync(category1Id);
        var category2 = await context.Categories.FindAsync(category2Id);
        
        Assert.Equal("Updated Category1", category1!.Name);
        Assert.Equal("Category2", category2!.Name);
    }

    [Fact]
    public async Task WhenUpdatingCategory_ThenLogsInformation()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Food"
        };
        
        await repository.CreateAsync(category);

        // Act
        var updatedCategory = new Category
        {
            Id = categoryId,
            Name = "Updated Food"
        };
        await repository.UpdateAsync(updatedCategory);

        // Assert
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Updating category")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
