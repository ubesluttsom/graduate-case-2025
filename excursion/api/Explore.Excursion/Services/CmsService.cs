using System;
using System.Net.Http;
using System.Threading.Tasks;
using Explore.Excursion.Exceptions;
using Explore.Excursion.Models;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Newtonsoft.Json;

namespace Explore.Excursion.Services;

public class CmsService : ICmsService
{
    private readonly HttpClient _httpClient;

    public CmsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> HealthCheckAsync()
    {
        var response = await _httpClient.GetAsync("health");

        if (!response.IsSuccessStatusCode)
            return "CMS is not healthy";

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<GuestResponse> CreateGuestAsync(CreateGuestRequest createGuest)
    {
        return await PostAsync<GuestResponse>("guests", createGuest);
    }

    public async Task<GuestResponse> GetGuestByIdAsync(Guid userId)
    {
        return await GetAsync<GuestResponse>($"guests/{userId}");
    }

    public async Task<RoomResponse> GetRoomByIdAsync(Guid roomId)
    {
        return await GetAsync<RoomResponse>($"rooms/{roomId}");
    }

    private async Task<T> GetAsync<T>(string url) where T : new()
    {
        var response = await _httpClient.GetAsync(url);

        var statusCode = (int)response.StatusCode;

        if (statusCode is > 399 and < 500)
            return new T();

        if (!response.IsSuccessStatusCode)
            throw new CmsException(await response.Content.ReadAsStringAsync());

        return await response.Content.ReadAsAsync<T>();
    }

    private async Task<T> PostAsync<T>(string url, object body) where T : new()
    {
        var response = await _httpClient.PostAsJsonAsync(url, body);

        var statusCode = (int)response.StatusCode;
        if (statusCode is > 399 and < 500)
            return new T();

        if (!response.IsSuccessStatusCode)
            throw new CmsException(await response.Content.ReadAsStringAsync());

        return await response.Content.ReadAsAsync<T>();
    }

    public async Task<String> GetExcursionActivity()
    {

        var excursion = new ExcursionActivity
        {
            Id = 1,
            Name = "Whale safari",
            Description = "Dette er en testaktivtet"
        };

        return JsonConvert.SerializeObject(excursion); 

    }

    public async Task<String> GetExcursionActivities()
    {
        var excursion = new ExcursionActivity
        {
            Id = 0,
            Name = "WHALE SAFARI",
            Description = "See the great whales of the artic up close along our Whale Safari."
        };
        var excursion1 = new ExcursionActivity
        {
            Id = 1,
            Name = "DOG SLED",
            Description = "Experience dog sledding at Svalbard with our amazing guides!"
        };
        var excursion3 = new ExcursionActivity
        {
            Id = 2,
            Name = "RIB TRIP",
            Description = "Experience the artic up close in this high speed adventure."
        };
        var excursions = new[] { excursion, excursion1, excursion3 };

        return JsonConvert.SerializeObject(excursions);
    }

}
