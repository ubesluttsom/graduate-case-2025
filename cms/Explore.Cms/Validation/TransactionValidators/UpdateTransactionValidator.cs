using Explore.Cms.Models;
using FluentValidation;

namespace Explore.Cms.Validation.TransactionValidators;

public class UpdateTransactionValidator : AbstractValidator<GuestTransaction>
{
    public UpdateTransactionValidator()
    {
        RuleFor(t => t.Id).NotEqual(Guid.Empty).WithMessage("Transaction must have an id");
    }
}