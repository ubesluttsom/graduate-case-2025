using FluentValidation;
using MongoDB.Bson;

namespace Explore.Cms.Validation.RoomValidators;

public class UpdateRoomValidator : RoomValidator
{
    public UpdateRoomValidator()
    {
        RuleFor(r => r.Id).NotEqual(ObjectId.Empty).WithMessage("Room must have an id");
    }
}