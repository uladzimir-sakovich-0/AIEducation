using FinanceTracker.API.Models.Requests;
using FinanceTracker.API.Validators;
using FluentValidation.TestHelper;

namespace FinanceTracker.UnitTests.Validators;

/// <summary>
/// Tests for CategoryCreateRequestValidator
/// </summary>
public class CategoryCreateRequestValidatorTests
{
    private readonly CategoryCreateRequestValidator _validator;

    public CategoryCreateRequestValidatorTests()
    {
        _validator = new CategoryCreateRequestValidator();
    }

    [Fact]
    public void WhenNameIsEmpty_ThenValidationFails()
    {
        // Arrange
        var request = new CategoryCreateRequest { Name = "" };

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
        var request = new CategoryCreateRequest { Name = null! };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void WhenNameExceedsMaxLength_ThenValidationFails()
    {
        // Arrange
        var request = new CategoryCreateRequest { Name = new string('a', 101) }; // 101 characters

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Category name must not exceed 100 characters");
    }

    [Fact]
    public void WhenNameIsValid_ThenValidationPasses()
    {
        // Arrange
        var request = new CategoryCreateRequest { Name = "Food" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void WhenNameIsMaxLength_ThenValidationPasses()
    {
        // Arrange
        var request = new CategoryCreateRequest { Name = new string('a', 100) }; // exactly 100 characters

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
