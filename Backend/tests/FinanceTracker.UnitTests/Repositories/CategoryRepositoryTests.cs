using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Data;
using FinanceTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTracker.UnitTests.Repositories;

/// <summary>
/// Tests for CategoryRepository with user ownership validation
/// </summary>
public class CategoryRepositoryTests
{
    private readonly Guid _testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private readonly Guid _otherUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

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
            Name = "Food",
            UserId = _testUserId
        };

        // Act
        var result = await repository.CreateAsync(category, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal("Food", result.Name);
        Assert.Equal(_testUserId, result.UserId);
        
        // Verify it was actually saved to the database
        var savedCategory = await context.Categories.FindAsync(category.Id);
        Assert.NotNull(savedCategory);
        Assert.Equal("Food", savedCategory.Name);
        Assert.Equal(_testUserId, savedCategory.UserId);
    }

    [Fact]
    public async Task WhenUpdatingCategory_WithMatchingUserId_ThenCategoryIsUpdated()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Food",
            UserId = _testUserId
        };
        
       await repository.CreateAsync(category, CancellationToken.None);

        // Act
        var updatedCategory = new Category
        {
            Id = categoryId,
            Name = "Updated Food",
            UserId = _testUserId
        };
        var result = await repository.UpdateAsync(updatedCategory, _testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
        Assert.Equal("Updated Food", result.Name);
        Assert.Equal(_testUserId, result.UserId);
    }

    [Fact]
    public async Task WhenUpdatingCategory_WithDifferentUserId_ThenReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Food",
            UserId = _testUserId
        };
        
        await repository.CreateAsync(category, CancellationToken.None);

        // Act
        var updatedCategory = new Category
        {
            Id = categoryId,
            Name = "Hacked Food",
            UserId = _otherUserId
        };
        var result = await repository.UpdateAsync(updatedCategory, _otherUserId, CancellationToken.None);

        // Assert
        Assert.Null(result);
        
        // Verify original category was not modified
        var savedCategory = await context.Categories.FindAsync(categoryId);
        Assert.NotNull(savedCategory);
        Assert.Equal("Food", savedCategory.Name);
        Assert.Equal(_testUserId, savedCategory.UserId);
    }

    [Fact]
    public async Task WhenGettingAllCategories_ThenOnlyUserCategoriesAreReturned()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var category1 = new Category { Id = Guid.NewGuid(), Name = "User1 Food", UserId = _testUserId };
        var category2 = new Category { Id = Guid.NewGuid(), Name = "User1 Housing", UserId = _testUserId };
        var category3 = new Category { Id = Guid.NewGuid(), Name = "User2 Food", UserId = _otherUserId };
        
        await repository.CreateAsync(category1, CancellationToken.None);
        await repository.CreateAsync(category2, CancellationToken.None);
        await repository.CreateAsync(category3, CancellationToken.None);

        // Act
        var result = await repository.GetAllAsync(_testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal(_testUserId, c.UserId));
        Assert.Contains(result, c => c.Name == "User1 Food");
        Assert.Contains(result, c => c.Name == "User1 Housing");
        Assert.DoesNotContain(result, c => c.Name == "User2 Food");
    }

    [Fact]
    public async Task WhenGettingAllCategories_WithNoCategories_ThenEmptyListIsReturned()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);

        // Act
        var result = await repository.GetAllAsync(_testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithMatchingUserId_ThenCategoryIsDeletedAndReturnsTrue()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Food",
            UserId = _testUserId
        };
        
        await repository.CreateAsync(category, CancellationToken.None);

        // Act
        var result = await repository.DeleteAsync(categoryId, _testUserId, CancellationToken.None);

        // Assert
        Assert.True(result);
        
        // Verify it was actually deleted from the database
        var deletedCategory = await context.Categories.FindAsync(categoryId);
        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task WhenDeletingCategory_WithDifferentUserId_ThenReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var repository = new CategoryRepository(context, logger.Object);
        
        var categoryId = Guid.NewGuid();
        var category = new Category
        {
            Id = categoryId,
            Name = "Food",
            UserId = _testUserId
        };
        
        await repository.CreateAsync(category, CancellationToken.None);

        // Act
        var result = await repository.DeleteAsync(categoryId, _otherUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
        
        // Verify category still exists
        var savedCategory = await context.Categories.FindAsync(categoryId);
        Assert.NotNull(savedCategory);
        Assert.Equal("Food", savedCategory.Name);
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
        var result = await repository.DeleteAsync(nonExistentId, _testUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
