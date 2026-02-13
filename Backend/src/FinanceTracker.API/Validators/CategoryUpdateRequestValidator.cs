using FinanceTracker.Infrastructure.Models.Requests;
using FluentValidation;

namespace FinanceTracker.API.Validators;

/// <summary>
/// Validator for CategoryUpdateRequest
/// </summary>
public class CategoryUpdateRequestValidator : AbstractValidator<CategoryUpdateRequest>
{
    public CategoryUpdateRequestValidator()
    {
        // Id is required
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID is required");

        // Name is required (from Category entity [Required] attribute)
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required");

        // Name max length is 100 (from Category entity [MaxLength(100)] attribute)
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Category name must not exceed 100 characters");
    }
}
