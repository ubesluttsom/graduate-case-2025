using Explore.Cms.Models;
using FluentValidation;

namespace Explore.Cms.Validation.TransactionValidators;

public class CreateTransactionValidator : AbstractValidator<GuestTransaction>
{
    public CreateTransactionValidator()
    {
        RuleFor(t => t.Id).Equal(Guid.Empty).WithMessage("Transaction cannot have an id");
        RuleFor(t => t.RoomId).NotEqual(Guid.Empty).WithMessage("Transaction must have a room id");
    }
}