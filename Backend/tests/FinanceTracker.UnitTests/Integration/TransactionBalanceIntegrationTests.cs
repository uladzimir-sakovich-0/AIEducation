using FinanceTracker.DB.Entities;
using FinanceTracker.Infrastructure.Data;
using FinanceTracker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinanceTracker.UnitTests.Integration;

/// <summary>
/// Integration tests for verifying account balance updates when transactions are created, updated, or deleted
/// </summary>
public class TransactionBalanceIntegrationTests
{
    private readonly Guid _testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private FinanceTrackerDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<FinanceTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        return new FinanceTrackerDbContext(options);
    }

    private Mock<ILogger<TransactionRepository>> GetMockTransactionLogger()
    {
        return new Mock<ILogger<TransactionRepository>>();
    }

    private Mock<ILogger<AccountRepository>> GetMockAccountLogger()
    {
        return new Mock<ILogger<AccountRepository>>();
    }

    private async Task<Account> CreateTestAccount(FinanceTrackerDbContext context, Guid userId, decimal initialBalance = 100.00m)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = "Test Account",
            AccountType = "Checking",
            Balance = initialBalance,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return account;
    }

    private async Task<Category> CreateTestCategory(FinanceTrackerDbContext context, Guid userId)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            UserId = userId
        };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    [Fact]
    public async Task WhenCreatingPositiveTransaction_ThenBalanceIsIncreased()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var accountRepository = new AccountRepository(context, GetMockAccountLogger().Object);
        var transactionRepository = new TransactionRepository(context, GetMockTransactionLogger().Object, accountRepository);
        
        var account = await CreateTestAccount(context, _testUserId, initialBalance: 100.00m);
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
        await transactionRepository.CreateAsync(transaction, CancellationToken.None);

        // Assert
        var updatedAccount = await context.Accounts.FindAsync(account.Id);
        Assert.NotNull(updatedAccount);
        Assert.Equal(120.00m, updatedAccount.Balance);
    }

    [Fact]
    public async Task WhenCreatingNegativeTransaction_ThenBalanceIsDecreased()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var accountRepository = new AccountRepository(context, GetMockAccountLogger().Object);
        var transactionRepository = new TransactionRepository(context, GetMockTransactionLogger().Object, accountRepository);
        
        var account = await CreateTestAccount(context, _testUserId, initialBalance: 100.00m);
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
        await transactionRepository.CreateAsync(transaction, CancellationToken.None);

        // Assert
        var updatedAccount = await context.Accounts.FindAsync(account.Id);
        Assert.NotNull(updatedAccount);
        Assert.Equal(80.00m, updatedAccount.Balance);
    }

    [Fact]
    public async Task WhenUpdatingTransactionToLowerAmount_ThenBalanceIsDecreased()
    {
        // Arrange - Account with 100$, existing transaction of 50$
        using var context = GetInMemoryDbContext();
        var accountRepository = new AccountRepository(context, GetMockAccountLogger().Object);
        var transactionRepository = new TransactionRepository(context, GetMockTransactionLogger().Object, accountRepository);
        
        var account = await CreateTestAccount(context, _testUserId, initialBalance: 100.00m);
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
        await transactionRepository.CreateAsync(transaction, CancellationToken.None);

        // Verify balance after creating 50$ transaction: 100 + 50 = 150
        var accountAfterCreate = await context.Accounts.FindAsync(account.Id);
        Assert.Equal(150.00m, accountAfterCreate!.Balance);

        // Act - Update transaction from 50$ to 40$
        var updatedTransaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 40.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Updated transaction"
        };
        await transactionRepository.UpdateAsync(updatedTransaction, _testUserId, CancellationToken.None);

        // Assert - Balance should be 150 + (40 - 50) = 140
        var finalAccount = await context.Accounts.FindAsync(account.Id);
        Assert.NotNull(finalAccount);
        Assert.Equal(140.00m, finalAccount.Balance);
    }

    [Fact]
    public async Task WhenUpdatingTransactionToHigherAmount_ThenBalanceIsIncreased()
    {
        // Arrange - Account with 100$, existing transaction of 50$
        using var context = GetInMemoryDbContext();
        var accountRepository = new AccountRepository(context, GetMockAccountLogger().Object);
        var transactionRepository = new TransactionRepository(context, GetMockTransactionLogger().Object, accountRepository);
        
        var account = await CreateTestAccount(context, _testUserId, initialBalance: 100.00m);
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
        await transactionRepository.CreateAsync(transaction, CancellationToken.None);

        // Verify balance after creating 50$ transaction: 100 + 50 = 150
        var accountAfterCreate = await context.Accounts.FindAsync(account.Id);
        Assert.Equal(150.00m, accountAfterCreate!.Balance);

        // Act - Update transaction from 50$ to 80$
        var updatedTransaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 80.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Updated transaction"
        };
        await transactionRepository.UpdateAsync(updatedTransaction, _testUserId, CancellationToken.None);

        // Assert - Balance should be 150 + (80 - 50) = 180
        var finalAccount = await context.Accounts.FindAsync(account.Id);
        Assert.NotNull(finalAccount);
        Assert.Equal(180.00m, finalAccount.Balance);
    }

    [Fact]
    public async Task WhenDeletingTransaction_ThenBalanceIsReversed()
    {
        // Arrange - Account with 100$, existing transaction of 5$
        using var context = GetInMemoryDbContext();
        var accountRepository = new AccountRepository(context, GetMockAccountLogger().Object);
        var transactionRepository = new TransactionRepository(context, GetMockTransactionLogger().Object, accountRepository);
        
        var account = await CreateTestAccount(context, _testUserId, initialBalance: 100.00m);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            AccountId = account.Id,
            Amount = 5.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Transaction to delete"
        };
        await transactionRepository.CreateAsync(transaction, CancellationToken.None);

        // Verify balance after creating 5$ transaction: 100 + 5 = 105
        var accountAfterCreate = await context.Accounts.FindAsync(account.Id);
        Assert.Equal(105.00m, accountAfterCreate!.Balance);

        // Act - Delete transaction
        await transactionRepository.DeleteAsync(transactionId, _testUserId, CancellationToken.None);

        // Assert - Balance should be 105 - 5 = 100
        var finalAccount = await context.Accounts.FindAsync(account.Id);
        Assert.NotNull(finalAccount);
        Assert.Equal(100.00m, finalAccount.Balance);
    }

    [Fact]
    public async Task WhenBalanceCanBeNegative_ThenNoExceptionIsThrown()
    {
        // Arrange - Account with 100$
        using var context = GetInMemoryDbContext();
        var accountRepository = new AccountRepository(context, GetMockAccountLogger().Object);
        var transactionRepository = new TransactionRepository(context, GetMockTransactionLogger().Object, accountRepository);
        
        var account = await CreateTestAccount(context, _testUserId, initialBalance: 100.00m);
        var category = await CreateTestCategory(context, _testUserId);
        
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = -150.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Large expense"
        };

        // Act
        await transactionRepository.CreateAsync(transaction, CancellationToken.None);

        // Assert - Balance should be negative: 100 - 150 = -50
        var updatedAccount = await context.Accounts.FindAsync(account.Id);
        Assert.NotNull(updatedAccount);
        Assert.Equal(-50.00m, updatedAccount.Balance);
    }

    [Fact]
    public async Task WhenMultipleTransactionsCreated_ThenBalanceIsAccurate()
    {
        // Arrange - Account with 100$
        using var context = GetInMemoryDbContext();
        var accountRepository = new AccountRepository(context, GetMockAccountLogger().Object);
        var transactionRepository = new TransactionRepository(context, GetMockTransactionLogger().Object, accountRepository);
        
        var account = await CreateTestAccount(context, _testUserId, initialBalance: 100.00m);
        var category = await CreateTestCategory(context, _testUserId);
        
        // Act - Create multiple transactions
        await transactionRepository.CreateAsync(new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = 50.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Income 1"
        }, CancellationToken.None);

        await transactionRepository.CreateAsync(new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = -30.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Expense 1"
        }, CancellationToken.None);

        await transactionRepository.CreateAsync(new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = 20.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = category.Id,
            Notes = "Income 2"
        }, CancellationToken.None);

        // Assert - Balance should be 100 + 50 - 30 + 20 = 140
        var finalAccount = await context.Accounts.FindAsync(account.Id);
        Assert.NotNull(finalAccount);
        Assert.Equal(140.00m, finalAccount.Balance);
    }
}
