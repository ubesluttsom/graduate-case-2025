using Explore.Cms.Models;
using FluentValidation;
using MongoDB.Bson;

namespace Explore.Cms.Validation.TransactionValidators;

public class CreateTransactionValidator : AbstractValidator<GuestTransaction>
{
    public CreateTransactionValidator()
    {
        RuleFor(t => t.Id).Equal(ObjectId.Empty).WithMessage("Transaction cannot have an id");
        RuleFor(t => t.RoomId).NotEqual(ObjectId.Empty).WithMessage("Transaction must have a room id");
    }
}