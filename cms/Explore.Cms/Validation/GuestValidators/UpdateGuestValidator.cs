using Explore.Cms.Models;
using FluentValidation;
using MongoDB.Bson;

namespace Explore.Cms.Validation.GuestValidators;

public class UpdateGuestValidator : AbstractValidator<Guest>
{
    public UpdateGuestValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty).WithMessage("Guest must have an id");
    }
}