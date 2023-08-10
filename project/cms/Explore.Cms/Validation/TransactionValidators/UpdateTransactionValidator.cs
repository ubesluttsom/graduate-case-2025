using Explore.Cms.Models;
using FluentValidation;
using MongoDB.Bson;

namespace Explore.Cms.Validation.TransactionValidators;

public class UpdateTransactionValidator : AbstractValidator<GuestTransaction>
{
    public UpdateTransactionValidator()
    {
        RuleFor(t => t.Id).NotEqual(ObjectId.Empty).WithMessage("Transaction must have an id");
    }
}