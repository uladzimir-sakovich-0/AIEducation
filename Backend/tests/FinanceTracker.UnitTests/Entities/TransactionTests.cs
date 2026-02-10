using FinanceTracker.DB.Entities;

namespace FinanceTracker.UnitTests.Entities;

/// <summary>
/// Tests for the Transaction entity
/// </summary>
public class TransactionTests
{
    [Fact]
    public void WhenTransactionIsCreated_ThenDefaultValuesAreSet()
    {
        // Arrange & Act
        var transaction = new Transaction();

        // Assert
        Assert.Equal(Guid.Empty, transaction.Id);
        Assert.Equal(Guid.Empty, transaction.AccountId);
        Assert.Equal(0m, transaction.Amount);
        Assert.Equal(Guid.Empty, transaction.CategoryId);
        Assert.Null(transaction.Notes);
    }

    [Fact]
    public void WhenTransactionPropertiesAreSet_ThenTheyCanBeRetrieved()
    {
        // Arrange
        var id = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var amount = 123.45m;
        var timestamp = DateTime.UtcNow;
        var categoryId = Guid.NewGuid();
        var notes = "Test transaction";

        // Act
        var transaction = new Transaction
        {
            Id = id,
            AccountId = accountId,
            Amount = amount,
            Timestamp = timestamp,
            CategoryId = categoryId,
            Notes = notes
        };

        // Assert
        Assert.Equal(id, transaction.Id);
        Assert.Equal(accountId, transaction.AccountId);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(timestamp, transaction.Timestamp);
        Assert.Equal(categoryId, transaction.CategoryId);
        Assert.Equal(notes, transaction.Notes);
    }

    [Fact]
    public void WhenTransactionWithoutNotes_ThenNotesCanBeNull()
    {
        // Arrange & Act
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Amount = 50.00m,
            CategoryId = Guid.NewGuid()
        };

        // Assert
        Assert.Null(transaction.Notes);
    }

    [Fact]
    public void WhenNegativeAmount_ThenItIsStored()
    {
        // Arrange & Act
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Amount = -75.50m,
            CategoryId = Guid.NewGuid()
        };

        // Assert
        Assert.Equal(-75.50m, transaction.Amount);
    }
}
