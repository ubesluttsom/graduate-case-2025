using System.Linq;
using Explore.Cms.Models;
using FluentValidation;

namespace Explore.Cms.Validation.RoomValidators;

public class RoomValidator : AbstractValidator<Room>
{
    protected RoomValidator()
    {
        RuleFor(r => r.GuestIds)
            .Must(ids => ids.Count == ids.Distinct().Count())
            .WithMessage("Room cannot have duplicate guests.");

        RuleFor(r => r.TransactionIds)
            .Must(ids => ids.Count == ids.Distinct().Count())
            .WithMessage("Room cannot have duplicate transactions.");
    }
}