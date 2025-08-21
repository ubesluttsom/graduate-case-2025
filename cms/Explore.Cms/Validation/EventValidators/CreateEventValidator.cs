using Explore.Cms.Models;
using FluentValidation;

namespace Explore.Cms.Validation.EventValidators;

public class CreateEventValidator : AbstractValidator<Events>
{
    public CreateEventValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(e => e.Description)
            .MaximumLength(500);

        RuleFor(e => e.AvailableSpots)
            .GreaterThan(0);
    }
}
