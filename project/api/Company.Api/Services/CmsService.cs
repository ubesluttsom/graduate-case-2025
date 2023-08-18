using System;
using System.Net.Http;
using System.Threading.Tasks;
using Company.Api.Exceptions;
using Company.Api.Models;

namespace Company.Api.Services;

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
    
    private async Task<T> PostAsync<T>(string url, object body)
    {
        var response = await _httpClient.PostAsJsonAsync(url, body);
        if (!response.IsSuccessStatusCode)
            throw new CmsException(await response.Content.ReadAsStringAsync());

        return await response.Content.ReadAsAsync<T>();
    }
}
