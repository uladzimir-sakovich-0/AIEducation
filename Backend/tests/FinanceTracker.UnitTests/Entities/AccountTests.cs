using FinanceTracker.DB.Entities;

namespace FinanceTracker.UnitTests.Entities;

/// <summary>
/// Tests for the Account entity (financial account)
/// </summary>
public class AccountTests
{
    [Fact]
    public void WhenAccountIsCreated_ThenDefaultValuesAreSet()
    {
        // Arrange & Act
        var account = new Account();

        // Assert
        Assert.Equal(Guid.Empty, account.Id);
        Assert.Equal(Guid.Empty, account.UserId);
        Assert.Equal(string.Empty, account.Name);
        Assert.Equal(string.Empty, account.AccountType);
        Assert.Equal(0m, account.Balance);
        Assert.NotNull(account.Transactions);
        Assert.Empty(account.Transactions);
    }

    [Fact]
    public void WhenAccountPropertiesAreSet_ThenTheyCanBeRetrieved()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var name = "Main Checking";
        var accountType = "Checking";
        var balance = 1500.75m;
        var createdAt = DateTime.UtcNow;

        // Act
        var account = new Account
        {
            Id = id,
            UserId = userId,
            Name = name,
            AccountType = accountType,
            Balance = balance,
            CreatedAt = createdAt
        };

        // Assert
        Assert.Equal(id, account.Id);
        Assert.Equal(userId, account.UserId);
        Assert.Equal(name, account.Name);
        Assert.Equal(accountType, account.AccountType);
        Assert.Equal(balance, account.Balance);
        Assert.Equal(createdAt, account.CreatedAt);
    }

    [Fact]
    public void WhenTransactionsAreAdded_ThenTheyAreAccessible()
    {
        // Arrange
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = "Savings",
            AccountType = "Savings"
        };
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = 250.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        account.Transactions.Add(transaction);

        // Assert
        Assert.Single(account.Transactions);
        Assert.Contains(transaction, account.Transactions);
    }

    [Fact]
    public void WhenNegativeBalance_ThenItIsStored()
    {
        // Arrange & Act
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = "Credit Card",
            AccountType = "Credit Card",
            Balance = -500.00m
        };

        // Assert
        Assert.Equal(-500.00m, account.Balance);
    }
}
