using FinanceTracker.DB.Entities;

namespace FinanceTracker.UnitTests.Entities;

/// <summary>
/// Tests for the User entity
/// </summary>
public class UserTests
{
    [Fact]
    public void WhenUserIsCreated_ThenDefaultValuesAreSet()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.Equal(Guid.Empty, user.Id);
        Assert.Equal(string.Empty, user.Email);
        Assert.Equal(string.Empty, user.PasswordHash);
        Assert.True(user.IsActive);
        Assert.NotNull(user.Accounts);
        Assert.Empty(user.Accounts);
    }

    [Fact]
    public void WhenUserPropertiesAreSet_ThenTheyCanBeRetrieved()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = "test@example.com";
        var passwordHash = "hashed_password";
        var isActive = false;
        var createdAt = DateTime.UtcNow;

        // Act
        var user = new User
        {
            Id = id,
            Email = email,
            PasswordHash = passwordHash,
            IsActive = isActive,
            CreatedAt = createdAt
        };

        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(email, user.Email);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(isActive, user.IsActive);
        Assert.Equal(createdAt, user.CreatedAt);
    }

    [Fact]
    public void WhenAccountsAreAdded_ThenTheyAreAccessible()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid() };
        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = "Main Checking",
            AccountType = "Checking",
            Balance = 1000.00m
        };

        // Act
        user.Accounts.Add(account);

        // Assert
        Assert.Single(user.Accounts);
        Assert.Contains(account, user.Accounts);
    }
}
