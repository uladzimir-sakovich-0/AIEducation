using FinanceTracker.API.Validators;
using FinanceTracker.Infrastructure.Models.Requests;
using FluentValidation.TestHelper;

namespace FinanceTracker.UnitTests.Validators;

/// <summary>
/// Tests for TransactionCreateRequestValidator
/// </summary>
public class TransactionCreateRequestValidatorTests
{
    private readonly TransactionCreateRequestValidator _validator;

    public TransactionCreateRequestValidatorTests()
    {
        _validator = new TransactionCreateRequestValidator();
    }

    [Fact]
    public void WhenAccountIdIsEmpty_ThenValidationFails()
    {
        // Arrange
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.Empty,
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = "Test transaction"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AccountId)
            .WithErrorMessage("Account ID is required");
    }

    [Fact]
    public void WhenAmountIsZero_ThenValidationFails()
    {
        // Arrange
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 0,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = "Test transaction"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Amount)
            .WithErrorMessage("Amount is required");
    }

    [Fact]
    public void WhenTimestampIsDefault_ThenValidationFails()
    {
        // Arrange
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m,
            Timestamp = default,
            CategoryId = Guid.NewGuid(),
            Notes = "Test transaction"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Timestamp)
            .WithErrorMessage("Timestamp is required");
    }

    [Fact]
    public void WhenCategoryIdIsEmpty_ThenValidationFails()
    {
        // Arrange
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.Empty,
            Notes = "Test transaction"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId)
            .WithErrorMessage("Category ID is required");
    }

    [Fact]
    public void WhenNotesExceedsMaxLength_ThenValidationFails()
    {
        // Arrange
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = new string('a', 501) // 501 characters
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Notes)
            .WithErrorMessage("Notes must not exceed 500 characters");
    }

    [Fact]
    public void WhenNotesIsNull_ThenValidationPasses()
    {
        // Arrange
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenNotesIsEmpty_ThenValidationPasses()
    {
        // Arrange
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = ""
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenNotesIsMaxLength_ThenValidationPasses()
    {
        // Arrange
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = new string('a', 500) // exactly 500 characters
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenRequestIsValid_ThenValidationPasses()
    {
        // Arrange
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 100.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = "Grocery shopping"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenAmountIsNegative_ThenValidationPasses()
    {
        // Arrange
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = -50.00m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = "Expense transaction"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenAmountIsPositive_ThenValidationPasses()
    {
        // Arrange
        var request = new TransactionCreateRequest
        {
            AccountId = Guid.NewGuid(),
            Amount = 250.50m,
            Timestamp = DateTime.UtcNow,
            CategoryId = Guid.NewGuid(),
            Notes = "Income transaction"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
