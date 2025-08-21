using Explore.Cms.Models;
using FluentValidation;

namespace Explore.Cms.Validation.EventValidators;

public class UpdateEventValidator : AbstractValidator<Events>
{
    public UpdateEventValidator()
    {
        RuleFor(e => e.Id)
            .NotEmpty();

        RuleFor(e => e.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(e => e.Description)
            .MaximumLength(500);

        RuleFor(e => e.AvailableSpots)
            .GreaterThan(0);
    }
}
