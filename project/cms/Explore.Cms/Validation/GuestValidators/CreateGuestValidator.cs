using Explore.Cms.Models;
using FluentValidation;

namespace Explore.Cms.Validation.GuestValidators;

public class CreateGuestValidator : AbstractValidator<Guest>
{
    public CreateGuestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("A guest must have a first name");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("A guest must have a last name");
    }
}