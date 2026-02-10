using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.UnitTests.Data;

/// <summary>
/// Tests for the FinanceTrackerDbContext
/// </summary>
public class FinanceTrackerDbContextTests
{
    private DbContextOptions<FinanceTrackerDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<FinanceTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task WhenAccountIsAdded_ThenCreatedAtIsSetAutomatically()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var beforeAdd = DateTime.UtcNow;

        using (var context = new FinanceTrackerDbContext(options))
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashed_password"
            };

            // Act
            context.Accounts.Add(account);
            await context.SaveChangesAsync();

            // Assert
            var afterAdd = DateTime.UtcNow;
            Assert.True(account.CreatedAt >= beforeAdd && account.CreatedAt <= afterAdd,
                "CreatedAt should be set to current UTC time");
        }
    }

    [Fact]
    public async Task WhenTransactionIsAdded_ThenTimestampIsSetAutomatically()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var beforeAdd = DateTime.UtcNow;

        using (var context = new FinanceTrackerDbContext(options))
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashed_password"
            };

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Food"
            };

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                CategoryId = category.Id,
                Amount = 100.00m
            };

            // Act
            context.Accounts.Add(account);
            context.Categories.Add(category);
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            // Assert
            var afterAdd = DateTime.UtcNow;
            Assert.True(transaction.Timestamp >= beforeAdd && transaction.Timestamp <= afterAdd,
                "Timestamp should be set to current UTC time");
        }
    }

    [Fact]
    public async Task WhenAccountWithExistingCreatedAtIsAdded_ThenCreatedAtIsPreserved()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var specificDate = DateTime.UtcNow.AddDays(-10);

        using (var context = new FinanceTrackerDbContext(options))
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashed_password",
                CreatedAt = specificDate
            };

            // Act
            context.Accounts.Add(account);
            await context.SaveChangesAsync();

            // Assert - The explicitly set CreatedAt should be preserved
            Assert.Equal(specificDate, account.CreatedAt);
        }
    }

    [Fact]
    public async Task WhenTransactionWithExistingTimestampIsAdded_ThenTimestampIsPreserved()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var specificDate = DateTime.UtcNow.AddHours(-5);

        using (var context = new FinanceTrackerDbContext(options))
        {
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashed_password"
            };

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Food"
            };

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = account.Id,
                CategoryId = category.Id,
                Amount = 100.00m,
                Timestamp = specificDate
            };

            // Act
            context.Accounts.Add(account);
            context.Categories.Add(category);
            context.Transactions.Add(transaction);
            await context.SaveChangesAsync();

            // Assert - The explicitly set Timestamp should be preserved
            Assert.Equal(specificDate, transaction.Timestamp);
        }
    }
}
