using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Explore.Cms.Helpers.Http;

public class ValidatableRequest<T> where T : new()
{
    /// <summary>
    /// The deserialized value of the request.
    /// </summary>
    public T Value { get; init; } = new();

    /// <summary>
    /// Whether or not the deserialized value was found to be valid.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// The collection of validation errors.
    /// </summary>
    public IEnumerable<ValidationFailure> Errors { get; init; } = new List<ValidationFailure>();

    // <summary Creates a <see cref="BadRequestObjectResult"/> containing a collection of minimal validation error details. </summary>
    /// <returns><see cref="BadRequestObjectResult" /></returns>
    public BadRequestObjectResult ToBadRequest()
    {
        return new BadRequestObjectResult(Errors.Select(e => new
        {
            Field = e.PropertyName,
            Error = e.ErrorMessage
        }));
    }
}