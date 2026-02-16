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

    [Fact]
    public async Task WhenGettingAllCategories_ThenAllCategoriesAreReturned()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var category1 = new Category { Id = Guid.NewGuid(), Name = "Food" };
        var category2 = new Category { Id = Guid.NewGuid(), Name = "Housing" };
        var category3 = new Category { Id = Guid.NewGuid(), Name = "Transportation" };
        
        await repository.CreateAsync(category1);
        await repository.CreateAsync(category2);
        await repository.CreateAsync(category3);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, c => c.Name == "Food");
        Assert.Contains(result, c => c.Name == "Housing");
        Assert.Contains(result, c => c.Name == "Transportation");
    }

    [Fact]
    public async Task WhenGettingAllCategories_WithNoCategories_ThenEmptyListIsReturned()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task WhenGettingAllCategories_ThenLogsInformation()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);

        // Act
        await repository.GetAllAsync();

        // Assert
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieving all categories")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithValidId_ThenCategoryIsDeletedAndReturnsTrue()
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
        var result = await repository.DeleteAsync(categoryId);

        // Assert
        Assert.True(result);
        
        // Verify it was actually deleted from the database
        var deletedCategory = await context.Categories.FindAsync(categoryId);
        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithInvalidId_ThenReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.DeleteAsync(nonExistentId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task WhenDeletingCategory_ThenOtherCategoriesRemainUnaffected()
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
        await repository.DeleteAsync(category1Id);

        // Assert
        var remainingCategories = await repository.GetAllAsync();
        Assert.Single(remainingCategories);
        Assert.Equal("Category2", remainingCategories[0].Name);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithValidId_ThenLogsInformation()
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
        await repository.DeleteAsync(categoryId);

        // Assert
        logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("deleted successfully")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithInvalidId_ThenLogsWarning()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var nonExistentId = Guid.NewGuid();

        // Act
        await repository.DeleteAsync(nonExistentId);

        // Assert
        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("not found for deletion")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
