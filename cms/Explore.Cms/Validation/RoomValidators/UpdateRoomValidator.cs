using FluentValidation;

namespace Explore.Cms.Validation.RoomValidators;

public class UpdateRoomValidator : RoomValidator
{
    public UpdateRoomValidator()
    {
        RuleFor(r => r.Id).NotEqual(Guid.Empty).WithMessage("Room must have an id");
    }
}