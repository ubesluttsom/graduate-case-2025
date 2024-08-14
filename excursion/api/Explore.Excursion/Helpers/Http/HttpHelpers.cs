using System;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace Explore.Excursion.Helpers.Http;


public static class HttpHelpers
{
    public static async Task<T> ValidateRequest<T, TV>(HttpRequest req)
        where TV : AbstractValidator<T>, new()
        where T : new()
    {
        T jsonBody;

        try
        {
            jsonBody = await GetJsonBody<T>(req);
        }
        catch (Exception ex) when (ex is FormatException or JsonReaderException or JsonSerializationException)
        {
            throw new ValidationException("Cannot parse json");
        }

        var validator = new TV();
        await validator.ValidateAndThrowAsync(jsonBody);
        return jsonBody;
    }
    
    public static ObjectResult GetObjectResult(HttpStatusCode statusCode, object? value = null)
    {
        return new ObjectResult(value)
        {
            StatusCode = (int)statusCode
        };
    }
    
    private static async Task<T> GetJsonBody<T>(HttpRequest req) where T : new()
    {
        var deserializedObject = JsonConvert.DeserializeObject<T>(await req.ReadAsStringAsync());
        if (deserializedObject == null)
            throw new JsonReaderException("Json body is empty");
        
        return deserializedObject;
    }
}