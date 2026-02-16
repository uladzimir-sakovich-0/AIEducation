using FinanceTracker.Infrastructure.Models.Requests;
using FluentValidation;

namespace FinanceTracker.API.Validators;

/// <summary>
/// Validator for TransactionCreateRequest
/// </summary>
public class TransactionCreateRequestValidator : AbstractValidator<TransactionCreateRequest>
{
    public TransactionCreateRequestValidator()
    {
        // AccountId is required (from Transaction entity [Required] attribute)
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID is required");

        // Amount is required (from Transaction entity [Required] attribute)
        RuleFor(x => x.Amount)
            .NotEmpty()
            .WithMessage("Amount is required");

        // Amount cannot be zero
        RuleFor(x => x.Amount)
            .NotEqual(0)
            .WithMessage("Amount cannot be zero");

        // Amount must be at least 1 penny in absolute value
        RuleFor(x => x.Amount)
            .Must(amount => Math.Abs(amount) >= 0.01m)
            .WithMessage("Amount must be at least 1 penny (0.01) in absolute value");

        // Timestamp is required (from Transaction entity [Required] attribute)
        RuleFor(x => x.Timestamp)
            .NotEmpty()
            .WithMessage("Timestamp is required");

        // CategoryId is required (from Transaction entity [Required] attribute)
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID is required");

        // Notes max length is 500 (from Transaction entity [MaxLength(500)] attribute)
        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes must not exceed 500 characters");
    }
}
