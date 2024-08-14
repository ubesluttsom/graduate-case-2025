using System.Net;
using System.Net.Http.Headers;
using Explore.Excursion.Exceptions;
using Explore.Excursion.Models;
using Explore.Excursion.Services;
using Explore.Excursion.Test.TestUtils;
using Newtonsoft.Json;
using NSubstitute;

namespace Explore.Excursion.Test.CmsService;

public class CmsServiceTests
{
    private readonly ICmsService _cmsService;
    private readonly MockHttpMessageHandler _handler;
    private const string JsonContentType = "application/json";
    
    public CmsServiceTests()
    {
        _handler = Substitute.ForPartsOf<MockHttpMessageHandler>();

        var httpClient = new HttpClient(_handler)
        {
            BaseAddress = new Uri("https://thisurldoesnotexist.com.no")
        };
        
        _cmsService = new Services.CmsService(httpClient);
    }
    
    [Fact]
    public async Task HealthCheckAsync_WhenCmsIsHealthy_ReturnsHealthy()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Healthy")
        };

        _handler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(response);
        
        // Act
        var result = await _cmsService.HealthCheckAsync();
        
        // Assert
        Assert.Equal("Healthy", result);
    }

    [Fact]
    public async Task HealthCheckAsync_WhenCmsIsNotHealthy_ReturnsNotHealthy()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        
        _handler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(response);
        
        // Act
        var result = await _cmsService.HealthCheckAsync();
        
        // Assert
        Assert.Equal("CMS is not healthy", result);
    }
    
    [Fact]
    public async Task CreateGuestAsync_WhenCmsIsHealthy_ReturnsGuest()
    {
        // Arrange
        var response = CreateResponse(new GuestResponse(), JsonContentType);
        
        _handler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(response);
        
        // Act
        var result = await _cmsService.CreateGuestAsync(new CreateGuestRequest());
        
        // Assert
        Assert.IsType<GuestResponse>(result);
    }
    
    [Fact]
    public async Task CreateGuestAsync_WhenCmsIsNotHealthy_ThrowsCmsError()
    {
        // Arrange
        var response = CreateResponse(statusCode: HttpStatusCode.InternalServerError);
        
        _handler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(response);
        
        // Assert and Act
        await Assert.ThrowsAsync<CmsException>(() => _cmsService.CreateGuestAsync(new CreateGuestRequest()));
    }
    
    [Fact]
    public async Task CreateGuestAsync_WhenBadCmsRequest_ReturnsDefaultGuest()
    {
        // Arrange
        var response = CreateResponse(statusCode: HttpStatusCode.BadRequest);
        var expectedResponse = new GuestResponse();
        
        _handler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(response);
        
        // Act
        var result = await _cmsService.CreateGuestAsync(new CreateGuestRequest());
        
        // Assert
        Assert.IsType<GuestResponse>(result);
        Assert.Equal(expectedResponse.Id, result.Id);
    }
    
    [Fact]
    public async Task GetGuestByIdAsync_WhenCmsIsHealthy_ReturnsGuest()
    {
        // Arrange
        var response = CreateResponse(new GuestResponse(), JsonContentType);

        _handler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(response);
        
        // Act
        var result = await _cmsService.GetGuestByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.IsType<GuestResponse>(result);
    }
    
    [Fact]
    public async Task GetGuestByIdAsync_WhenCmsIsNotHealthy_ThrowsCmsError()
    {
        // Arrange
        var response = CreateResponse(statusCode: HttpStatusCode.InternalServerError);
        
        _handler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(response);
        
        // Assert and Act
        await Assert.ThrowsAsync<CmsException>(() => _cmsService.GetGuestByIdAsync(Guid.NewGuid()));
    }
    
    [Fact]
    public async Task GetGuestByIdAsync_WhenBadCmsRequest_ReturnsDefaultGuest()
    {
        // Arrange
        var response = CreateResponse(statusCode: HttpStatusCode.BadRequest);
        var expectedResponse = new GuestResponse();
        
        _handler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(response);
        
        // Act
        var result = await _cmsService.GetGuestByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.IsType<GuestResponse>(result);
        Assert.Equal(expectedResponse.Id, result.Id);
    }
    
    [Fact]
    public async Task GetRoomByIdAsync_WhenCmsIsHealthy_ReturnsRoom()
    {
        // Arrange
        var response = CreateResponse(new RoomResponse(), JsonContentType);

        _handler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(response);
        
        // Act
        var result = await _cmsService.GetRoomByIdAsync(Guid.NewGuid());
        
        // Assert
        Assert.IsType<RoomResponse>(result);
    }
    
    [Fact]
    public async Task GetRoomByIdAsync_WhenCmsIsNotHealthy_ThrowsCmsError()
    {
        // Arrange
        var response = CreateResponse(statusCode: HttpStatusCode.InternalServerError);
        
        _handler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(response);
        
        // Assert and Act
        await Assert.ThrowsAsync<CmsException>(() => _cmsService.GetRoomByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetRoomByIdAsync_WhenBadCmsRequest_ReturnsDefaultRoom()
    {
        // Arrange
        var response = CreateResponse(statusCode: HttpStatusCode.BadRequest);
        var expectedResponse = new RoomResponse();

        _handler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(response);

        // Act
        var result = await _cmsService.GetRoomByIdAsync(Guid.NewGuid());

        // Assert
        Assert.IsType<RoomResponse>(result);
        Assert.Equal(expectedResponse.Id, result.Id);
    }

    private HttpResponseMessage CreateResponse(object? content = null, string contentType = "string/plain", HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var res = new HttpResponseMessage(statusCode);

        if (content != null)
            res.Content = new StringContent(JsonConvert.SerializeObject(content));

        res.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        return res;
    }
}