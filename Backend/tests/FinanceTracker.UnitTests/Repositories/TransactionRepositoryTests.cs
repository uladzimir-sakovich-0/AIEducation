using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Data;
using FinanceTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTracker.UnitTests.Repositories;

/// <summary>
/// Tests for TransactionRepository with user ownership validation
/// </summary>
public class TransactionRepositoryTests
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

    private Mock<ILogger<TransactionRepository>> GetMockLogger()
    {
        return new Mock<ILogger<TransactionRepository>>();
    }

    private Mock<IAccountRepository> GetMockAccountRepository()
    {
        var mock = new Mock<IAccountRepository>();
        // Default setup: UpdateBalanceAsync always succeeds
        mock.Setup(r => r.UpdateBalanceAsync(It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        return mock;
    }

    private async Task<Account> CreateTestAccount(FinanceTrackerDbContext context, Guid userId, string name = "Test Account")
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = name,
            AccountType = "Checking",
            Balance = 1000.00m,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return account;
    }

    private async Task<Category> CreateTestCategory(FinanceTrackerDbContext context, Guid userId, string name = "Test Category")
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            UserId = userId
        };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    [Fact]
    public async Task WhenCreatingTransaction_ThenTransactionIsSavedWithId()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = 100.50m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Grocery shopping"
        };

        // Act
        var result = await repository.CreateAsync(transaction, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transaction.Id, result.Id);
        Assert.Equal(100.50m, result.Amount);
        Assert.Equal(account.Id, result.AccountId);
        Assert.Equal(category.Id, result.CategoryId);
        Assert.Equal("Grocery shopping", result.Notes);
        
        // Verify it was actually saved to the database
        var savedTransaction = await context.Transactions.FindAsync(transaction.Id);
        Assert.NotNull(savedTransaction);
        Assert.Equal(100.50m, savedTransaction.Amount);
    }

    [Fact]
    public async Task WhenUpdatingTransaction_WithMatchingUserId_ThenTransactionIsUpdated()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Original note"
        };
        
        await repository.CreateAsync(transaction, CancellationToken.None);

        // Act
        var updatedTransaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 150.75m,
            Timestamp = DateTime.UtcNow.AddDays(1),
            CategoryId = category.Id,
            Notes = "Updated note"
        };
        var result = await repository.UpdateAsync(updatedTransaction, _testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.Id);
        Assert.Equal(150.75m, result.Amount);
        Assert.Equal("Updated note", result.Notes);
    }

    [Fact]
    public async Task WhenUpdatingTransaction_WithDifferentUserId_ThenReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Original note"
        };
        
        await repository.CreateAsync(transaction, CancellationToken.None);

        // Act
        var updatedTransaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 999.99m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Hacked note"
        };
        var result = await repository.UpdateAsync(updatedTransaction, _otherUserId, CancellationToken.None);

        // Assert
        Assert.Null(result);
        
        // Verify original transaction was not modified
        var savedTransaction = await context.Transactions.FindAsync(transactionId);
        Assert.NotNull(savedTransaction);
        Assert.Equal(100.00m, savedTransaction.Amount);
        Assert.Equal("Original note", savedTransaction.Notes);
    }

    [Fact]
    public async Task WhenGettingAllTransactions_ThenOnlyUserTransactionsAreReturned()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var user1Account = await CreateTestAccount(context, _testUserId, "User1 Account");
        var user2Account = await CreateTestAccount(context, _otherUserId, "User2 Account");
        var category = await CreateTestCategory(context, _testUserId);
        
        var transaction1 = new Transaction { Id = Guid.NewGuid(), AccountId = user1Account.Id, Amount = 100.00m, Timestamp = DateTime.UtcNow, CategoryId = category.Id, Notes = "User1 Transaction1" };
        var transaction2 = new Transaction { Id = Guid.NewGuid(), AccountId = user1Account.Id, Amount = 200.00m, Timestamp = DateTime.UtcNow, CategoryId = category.Id, Notes = "User1 Transaction2" };
        var transaction3 = new Transaction { Id = Guid.NewGuid(), AccountId = user2Account.Id, Amount = 300.00m, Timestamp = DateTime.UtcNow, CategoryId = category.Id, Notes = "User2 Transaction" };
        
        await repository.CreateAsync(transaction1, CancellationToken.None);
        await repository.CreateAsync(transaction2, CancellationToken.None);
        await repository.CreateAsync(transaction3, CancellationToken.None);

        // Act
        var result = await repository.GetAllAsync(_testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.Notes == "User1 Transaction1");
        Assert.Contains(result, t => t.Notes == "User1 Transaction2");
        Assert.DoesNotContain(result, t => t.Notes == "User2 Transaction");
    }

    [Fact]
    public async Task WhenGettingAllTransactions_WithNoTransactions_ThenEmptyListIsReturned()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);

        // Act
        var result = await repository.GetAllAsync(_testUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task WhenDeletingTransaction_WithMatchingUserId_ThenTransactionIsDeletedAndReturnsTrue()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "To be deleted"
        };
        
        await repository.CreateAsync(transaction, CancellationToken.None);

        // Act
        var result = await repository.DeleteAsync(transactionId, _testUserId, CancellationToken.None);

        // Assert
        Assert.True(result);
        
        // Verify it was actually deleted from the database
        var deletedTransaction = await context.Transactions.FindAsync(transactionId);
        Assert.Null(deletedTransaction);
    }

    [Fact]
    public async Task WhenDeletingTransaction_WithDifferentUserId_ThenReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Should not be deleted"
        };
        
        await repository.CreateAsync(transaction, CancellationToken.None);

        // Act
        var result = await repository.DeleteAsync(transactionId, _otherUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
        
        // Verify transaction still exists
        var savedTransaction = await context.Transactions.FindAsync(transactionId);
        Assert.NotNull(savedTransaction);
        Assert.Equal("Should not be deleted", savedTransaction.Notes);
    }

    [Fact]
    public async Task WhenDeletingTransaction_WithInvalidId_ThenReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.DeleteAsync(nonExistentId, _testUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task WhenValidatingAccountOwnership_WithMatchingUserId_ThenReturnsTrue()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);

        // Act
        var result = await repository.ValidateAccountOwnershipAsync(account.Id, _testUserId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task WhenValidatingAccountOwnership_WithDifferentUserId_ThenReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);

        // Act
        var result = await repository.ValidateAccountOwnershipAsync(account.Id, _otherUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task WhenValidatingAccountOwnership_WithNonExistentAccount_ThenReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var nonExistentAccountId = Guid.NewGuid();

        // Act
        var result = await repository.ValidateAccountOwnershipAsync(nonExistentAccountId, _testUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task WhenValidatingCategoryOwnership_WithMatchingUserId_ThenReturnsTrue()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var category = await CreateTestCategory(context, _testUserId);

        // Act
        var result = await repository.ValidateCategoryOwnershipAsync(category.Id, _testUserId, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task WhenValidatingCategoryOwnership_WithDifferentUserId_ThenReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var category = await CreateTestCategory(context, _testUserId);

        // Act
        var result = await repository.ValidateCategoryOwnershipAsync(category.Id, _otherUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task WhenValidatingCategoryOwnership_WithNonExistentCategory_ThenReturnsFalse()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var nonExistentCategoryId = Guid.NewGuid();

        // Act
        var result = await repository.ValidateCategoryOwnershipAsync(nonExistentCategoryId, _testUserId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task WhenCreatingTransaction_ThenAccountBalanceIsUpdated()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = 20.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Income transaction"
        };

        // Act
        await repository.CreateAsync(transaction, CancellationToken.None);

        // Assert
        accountRepository.Verify(r => r.UpdateBalanceAsync(account.Id, 20.00m, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenCreatingNegativeTransaction_ThenAccountBalanceIsDecreased()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = -20.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Expense transaction"
        };

        // Act
        await repository.CreateAsync(transaction, CancellationToken.None);

        // Assert
        accountRepository.Verify(r => r.UpdateBalanceAsync(account.Id, -20.00m, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenUpdatingTransactionAmount_ThenAccountBalanceIsAdjusted()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 50.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Original transaction"
        };
        
        await repository.CreateAsync(transaction, CancellationToken.None);
        accountRepository.Invocations.Clear(); // Clear previous invocations

        // Act - Update amount from 50 to 40
        var updatedTransaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 40.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Updated transaction"
        };
        await repository.UpdateAsync(updatedTransaction, _testUserId, CancellationToken.None);

        // Assert - Balance should be adjusted by (40 - 50) = -10
        accountRepository.Verify(r => r.UpdateBalanceAsync(account.Id, -10.00m, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenUpdatingTransactionAmountToHigher_ThenAccountBalanceIsIncreasedByDifference()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 50.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Original transaction"
        };
        
        await repository.CreateAsync(transaction, CancellationToken.None);
        accountRepository.Invocations.Clear(); // Clear previous invocations

        // Act - Update amount from 50 to 80
        var updatedTransaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 80.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Updated transaction"
        };
        await repository.UpdateAsync(updatedTransaction, _testUserId, CancellationToken.None);

        // Assert - Balance should be adjusted by (80 - 50) = 30
        accountRepository.Verify(r => r.UpdateBalanceAsync(account.Id, 30.00m, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenDeletingTransaction_ThenAccountBalanceIsReversed()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 5.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "To be deleted"
        };
        
        await repository.CreateAsync(transaction, CancellationToken.None);
        accountRepository.Invocations.Clear(); // Clear previous invocations

        // Act
        await repository.DeleteAsync(transactionId, _testUserId, CancellationToken.None);

        // Assert - Balance should be reversed by -5.00
        accountRepository.Verify(r => r.UpdateBalanceAsync(account.Id, -5.00m, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenDeletingNegativeTransaction_ThenAccountBalanceIsIncreasedByAbsoluteValue()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var logger = GetMockLogger();
        var accountRepository = GetMockAccountRepository();
        var repository = new TransactionRepository(context, logger.Object, accountRepository.Object);
        
        var account = await CreateTestAccount(context, _testUserId);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = -50.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Expense to be deleted"
        };
        
        await repository.CreateAsync(transaction, CancellationToken.None);
        accountRepository.Invocations.Clear(); // Clear previous invocations

        // Act
        await repository.DeleteAsync(transactionId, _testUserId, CancellationToken.None);

        // Assert - Balance should be reversed by -(-50.00) = 50.00
        accountRepository.Verify(r => r.UpdateBalanceAsync(account.Id, 50.00m, It.IsAny<CancellationToken>()), Times.Once);
    }
}
