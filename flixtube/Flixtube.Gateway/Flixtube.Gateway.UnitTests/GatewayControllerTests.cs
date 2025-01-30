using FluentAssertions;
using Moq;
using Moq.Protected;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;
using Flixtube.Gateway.Models;
using Flixtube.Gateway.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

namespace Flixtube.Gateway.UnitTests;

public class GatewayControllerTests
{
    private readonly List<Video> _videos;
    private readonly string _videos_json;
    private readonly Video _video1;
    private readonly string _video1_json;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<ILogger<GatewayController>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly GatewayController _gatewayController;
    private readonly ITestOutputHelper _output;

    public GatewayControllerTests(ITestOutputHelper output)
    {
        _output = output;

        // Test Fixture

        // Sample data
        _videos = new()
        {
            new Video() { Id = "5d9e690ad76fe06a3d7ae416", Name = "SampleVideo_1280x720_1mb.mp4" },
            new Video() { Id = "1asd098f9a00sf98f098asd9", Name = "SampleVideo2_1280x720_1mb.mp4" }
        };
        _video1 = _videos[0];
        string Id = _video1.Id;
        _videos_json = JsonSerializer.Serialize(_videos);
        _video1_json = JsonSerializer.Serialize(_video1);

        // Sample HTTP requests/responses
        var getVideosRequest = new HttpRequestMessage(HttpMethod.Get, "http://metadata/videos");
        var getVideosResponse = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(_videos_json) };
        var getVideo1Request = new HttpRequestMessage(HttpMethod.Get, $"http://metadata/video/{Id}");
        var getVideo1Response = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(_video1_json) };
        var postVideo1Request = new HttpRequestMessage(HttpMethod.Post, $"http://metadata/video");
        postVideo1Request.Content = new StringContent(_video1_json);
        var postVideo1Response = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(_video1_json) };
        var deleteVideo1Request = new HttpRequestMessage(HttpMethod.Delete, $"http://metadata/video/{Id}");
        var deleteVideo1Response = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK };
        
        // Mock HttpMessageHandler (for async requests)
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        // Mock GetVideos Request/Response
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                // ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == getVideosRequest.Method &&
                    req.RequestUri == getVideosRequest.RequestUri &&
                    req.Content == getVideosRequest.Content
                ),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(getVideosResponse);

        // Mock GetVideo(string id) Request/Response
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                // ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == getVideo1Request.Method &&
                    req.RequestUri == getVideo1Request.RequestUri &&
                    req.Content == getVideo1Request.Content
                ),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(getVideo1Response);

        // Mock AddVideo(Video video) Request/Response
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                // ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == postVideo1Request.Method &&
                    req.RequestUri == postVideo1Request.RequestUri &&
                    req.Content != null &&
                    req.Content.ReadAsStringAsync().Result.Contains(_video1.Id.ToString()) // Check content
                ),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(postVideo1Response);

        // Mock DeletVideo(string id) Request/Response
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                // ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == deleteVideo1Request.Method &&
                    req.RequestUri == deleteVideo1Request.RequestUri &&
                    req.Content == deleteVideo1Request.Content
                ),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(postVideo1Response);
            
        // Mock HTTPClient
        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://metadata:80")
        };

        // Mock HTTPClientFactory
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockHttpClientFactory
            // .Setup(factory => factory.CreateClient(It.IsAny<string>()))
            .Setup(factory => factory.CreateClient("MetadataClient"))
            .Returns(mockHttpClient);

        // Mock Logger
        _mockLogger = new Mock<ILogger<GatewayController>>();
        _mockLogger.Setup(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>())
        );

        // Mock Configuration
        _mockConfig = new Mock<IConfiguration>();
        // _mockConfig.Setup(c => c["MyEnvironmentVariable"]).Returns("MockedValue");

        // Subject Under Test (SUT)
        _gatewayController = new GatewayController(_mockLogger.Object, _mockConfig.Object, _mockHttpClientFactory.Object);
    }

    [Fact]
    public async Task GetMetadata_ForExistingMetadata_ShouldReturnMetadata()
    {
        // Arrange
        List<Video> expected = _videos;

        // Act
        IActionResult result = await _gatewayController.GetMetadata();

        // Assert

        // Verify correct status code 
        OkObjectResult ok = (OkObjectResult)result;
        ok.StatusCode.Should().Be(200);

        // Verify correct response
        List<Video> actual = (List<Video>)ok.Value!;
        actual.Count.Should().Be(expected.Count);
        for (int idx=0; idx < actual.Count; idx++)
        {
            actual[idx].Id.Should().Be(expected[idx].Id);
            actual[idx].Name.Should().Be(expected[idx].Name);
        }  
        _output.WriteLine($"GetMetadata(): Count={actual.Count}");
    }

    [Fact]
    public async Task GetMetadata_ForExistingId_ShouldReturnMetadata()
    {
        // Arrange
        string id = _video1.Id;
        Video expected = _video1;

        // Act
        IActionResult result = await _gatewayController.GetMetadata(id);

        // Assert

        // Verify correct status code 
        OkObjectResult ok = (OkObjectResult)result;
        ok.StatusCode.Should().Be(200);

        // Verify correct response
        Video actual = (Video)ok.Value!;
        actual.Id.Should().Be(expected.Id);
        actual.Name.Should().Be(expected.Name);
        _output.WriteLine($"GetMetadata({id}): Id={actual.Id}, Name={actual.Name}");
    }

    [Fact]
    public async Task AddMetadata_ForNonExistingId_ShouldReturnNewlyCreatedMetadata()
    {
        // Arrange
        string id = _video1.Id;
        Video expected = _video1;

        // Act
        IActionResult result = await _gatewayController.AddMetadata(expected);

        // Assert

        // Verify correct status code 
        OkObjectResult ok = (OkObjectResult)result;
        ok.StatusCode.Should().Be(200);

        // Verify correct response
        HttpResponseMessage responseMessage = (HttpResponseMessage)ok.Value!;
        var responseContent = await responseMessage.Content.ReadAsStringAsync();
        Video? actual = JsonSerializer.Deserialize<Video>(responseContent);
        actual?.Id.Should().Be(expected.Id);
        actual?.Name.Should().Be(expected.Name);
        _output.WriteLine($"AddMetadata({id}): Id={actual?.Id}, Name={actual?.Name}");
    }

    [Fact]
    public async Task DeleteMetadata_ForExistingId_ShouldReturnDeletedMetadata()
    {
        // Arrange
        string id = _video1.Id;
        Video expected = _video1;

        // Act
        IActionResult result = await _gatewayController.DeleteMetadata(id);

        // Assert

        // Verify correct status code 
        OkResult ok = (OkResult)result;
        ok.StatusCode.Should().Be(200);
        _output.WriteLine($"DeleteMetadata({id}): Id={expected.Id}, Name={expected.Name}");
    }
}