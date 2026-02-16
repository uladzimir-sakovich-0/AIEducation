using FinanceTracker.Infrastructure.Models.Requests;
using FluentValidation;

namespace FinanceTracker.API.Validators;

/// <summary>
/// Validator for AccountUpdateRequest
/// </summary>
public class AccountUpdateRequestValidator : AbstractValidator<AccountUpdateRequest>
{
    public AccountUpdateRequestValidator()
    {
        // Id is required
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Account ID is required");

        // Name is required (from Account entity [Required] attribute)
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Account name is required");

        // Name max length is 100 (from Account entity [MaxLength(100)] attribute)
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Account name must not exceed 100 characters");

        // AccountType is required (from Account entity [Required] attribute)
        RuleFor(x => x.AccountType)
            .NotEmpty()
            .WithMessage("Account type is required");

        // AccountType max length is 50 (from Account entity [MaxLength(50)] attribute)
        RuleFor(x => x.AccountType)
            .MaximumLength(50)
            .WithMessage("Account type must not exceed 50 characters");

        // AccountType must be either "Cash" or "Bank"
        RuleFor(x => x.AccountType)
            .Must(type => type == "Cash" || type == "Bank")
            .WithMessage("Account type must be either 'Cash' or 'Bank'");
    }
}
