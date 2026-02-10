using FinanceTracker.DB.Entities;

namespace FinanceTracker.UnitTests.Entities;

/// <summary>
/// Tests for the Account entity
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
        Assert.Equal(string.Empty, account.Email);
        Assert.Equal(string.Empty, account.PasswordHash);
        Assert.True(account.IsActive);
        Assert.NotNull(account.Transactions);
        Assert.Empty(account.Transactions);
    }

    [Fact]
    public void WhenAccountPropertiesAreSet_ThenTheyCanBeRetrieved()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = "test@example.com";
        var passwordHash = "hashed_password";
        var isActive = false;
        var createdAt = DateTime.UtcNow;

        // Act
        var account = new Account
        {
            Id = id,
            Email = email,
            PasswordHash = passwordHash,
            IsActive = isActive,
            CreatedAt = createdAt
        };

        // Assert
        Assert.Equal(id, account.Id);
        Assert.Equal(email, account.Email);
        Assert.Equal(passwordHash, account.PasswordHash);
        Assert.Equal(isActive, account.IsActive);
        Assert.Equal(createdAt, account.CreatedAt);
    }

    [Fact]
    public void WhenTransactionsAreAdded_ThenTheyAreAccessible()
    {
        // Arrange
        var account = new Account { Id = Guid.NewGuid() };
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Amount = 100.50m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        account.Transactions.Add(transaction);

        // Assert
        Assert.Single(account.Transactions);
        Assert.Contains(transaction, account.Transactions);
    }
}
