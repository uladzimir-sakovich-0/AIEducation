using FinanceTracker.DB.Entities;

namespace FinanceTracker.UnitTests.Entities;

/// <summary>
/// Tests for the Category entity
/// </summary>
public class CategoryTests
{
    [Fact]
    public void WhenCategoryIsCreated_ThenDefaultValuesAreSet()
    {
        // Arrange & Act
        var category = new Category();

        // Assert
        Assert.Equal(Guid.Empty, category.Id);
        Assert.Equal(string.Empty, category.Name);
        Assert.NotNull(category.Transactions);
        Assert.Empty(category.Transactions);
    }

    [Fact]
    public void WhenCategoryPropertiesAreSet_ThenTheyCanBeRetrieved()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Food";

        // Act
        var category = new Category
        {
            Id = id,
            Name = name
        };

        // Assert
        Assert.Equal(id, category.Id);
        Assert.Equal(name, category.Name);
    }

    [Fact]
    public void WhenTransactionsAreAdded_ThenTheyAreAccessible()
    {
        // Arrange
        var category = new Category { Id = Guid.NewGuid(), Name = "Entertainment" };
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            Amount = 50.00m,
            CategoryId = category.Id
        };

        // Act
        category.Transactions.Add(transaction);

        // Assert
        Assert.Single(category.Transactions);
        Assert.Contains(transaction, category.Transactions);
    }
}
