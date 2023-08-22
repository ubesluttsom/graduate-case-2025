using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using MongoDB.Bson;

namespace Explore.Cms.Helpers.Http;

public static class HttpRequestHelpers
{
    public static async Task<ValidatableRequest<T>> ValidateRequest<T, TV>(HttpRequest req)
        where TV : AbstractValidator<T>, new()
        where T : new()
    {
        T jsonBody;

        try
        {
            jsonBody = await GetJsonBody<T>(req);
        }
        catch (Exception ex) when (ex is FormatException or BsonException)
        {
            return new ValidatableRequest<T>
            {
                IsValid = false,
                Errors = new List<ValidationFailure>
                {
                    new("Json", ex.Message)
                }
            };
        }

        var validator = new TV();
        var validationResult = await validator.ValidateAsync(jsonBody);

        if (!validationResult.IsValid)
            return new ValidatableRequest<T>
            {
                Value = jsonBody,
                IsValid = false,
                Errors = validationResult.Errors
            };

        return new ValidatableRequest<T>
        {
            Value = jsonBody,
            IsValid = true
        };
    }

    private static async Task<T> GetJsonBody<T>(HttpRequest req) where T : new()
    {
        return JsonSerializer.Deserialize<T>(await req.ReadAsStringAsync(), new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        }) ?? new T();
    }
}