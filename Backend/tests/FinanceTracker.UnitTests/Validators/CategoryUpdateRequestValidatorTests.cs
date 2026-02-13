using FinanceTracker.API.Validators;
using FinanceTracker.Infrastructure.Models.Requests;
using FluentValidation.TestHelper;

namespace FinanceTracker.UnitTests.Validators;

/// <summary>
/// Tests for CategoryUpdateRequestValidator
/// </summary>
public class CategoryUpdateRequestValidatorTests
{
    private readonly CategoryUpdateRequestValidator _validator;

    public CategoryUpdateRequestValidatorTests()
    {
        _validator = new CategoryUpdateRequestValidator();
    }

    [Fact]
    public void WhenIdIsEmpty_ThenValidationFails()
    {
        // Arrange
        var request = new CategoryUpdateRequest { Id = Guid.Empty, Name = "Food" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Category ID is required");
    }

    [Fact]
    public void WhenNameIsEmpty_ThenValidationFails()
    {
        // Arrange
        var request = new CategoryUpdateRequest { Id = Guid.NewGuid(), Name = "" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Category name is required");
    }

    [Fact]
    public void WhenNameIsNull_ThenValidationFails()
    {
        // Arrange
        var request = new CategoryUpdateRequest { Id = Guid.NewGuid(), Name = null! };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void WhenNameExceedsMaxLength_ThenValidationFails()
    {
        // Arrange
        var request = new CategoryUpdateRequest 
        { 
            Id = Guid.NewGuid(), 
            Name = new string('a', 101) // 101 characters
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Category name must not exceed 100 characters");
    }

    [Fact]
    public void WhenRequestIsValid_ThenValidationPasses()
    {
        // Arrange
        var request = new CategoryUpdateRequest { Id = Guid.NewGuid(), Name = "Food" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenNameIsMaxLength_ThenValidationPasses()
    {
        // Arrange
        var request = new CategoryUpdateRequest 
        { 
            Id = Guid.NewGuid(), 
            Name = new string('a', 100) // exactly 100 characters
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
