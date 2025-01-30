using Microsoft.AspNetCore.Mvc;
using Flixtube.VideoStreaming.Services;

namespace Flixtube.VideoStreaming.Controllers;

[ApiController]
[Route("/")]
public class VideoStreamingController : ControllerBase
{
    private readonly ILogger<VideoStreamingController> _logger;
    private readonly IConfiguration _config;
    private readonly IRabbitMqPublisherService _rabbit;
    private readonly string VIDEO_STORAGE_SCHEME;
    private readonly string VIDEO_STORAGE_HOST;
    private readonly int VIDEO_STORAGE_PORT;
    
    public VideoStreamingController(ILogger<VideoStreamingController> logger, IConfiguration config, IRabbitMqPublisherService rabbit)
    {
        _logger = logger;
        _config = config;
        _rabbit = rabbit;

        VIDEO_STORAGE_SCHEME = config.GetValue<string>("VIDEO_STORAGE_SCHEME")!;
        VIDEO_STORAGE_HOST = config.GetValue<string>("VIDEO_STORAGE_HOST")!;
        VIDEO_STORAGE_PORT = config.GetValue<int>("VIDEO_STORAGE_PORT");

        _logger.LogInformation("VideoStreamingController() called.");
    }

    // Health check.
    [HttpGet("/health")]
    public async Task<IActionResult> Health()
    {
        await Task.Delay(0);
        return Ok();
    }

    // Stream video from the video storage microservice to the Flixtube.Gateway.
    [HttpGet("/video/{id}")]
    public async Task StreamVideo(string id)
    {
        _logger.LogInformation($"StreamVideo({id}) called.");

        // Forward request to the video storage microservice.
        HttpRequestMessage forwardRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{VIDEO_STORAGE_SCHEME}://{VIDEO_STORAGE_HOST}:{VIDEO_STORAGE_PORT}/video?id={id}")
        };
        Request.Headers.ToList().ForEach(header => forwardRequest.Headers.Add(header.Key, header.Value.ToString()));
        using HttpClient httpClient = new();
        HttpResponseMessage forwardResponse = await httpClient.SendAsync(forwardRequest);
        // _logger.LogInformation($"Success: {forwardResponse.IsSuccessStatusCode}");

        // Forward response from the video storage microservice.
        forwardResponse.Headers.ToList().ForEach(header => Response.Headers.Append(header.Key, header.Value.ToString()));
        Response.StatusCode = (int)forwardResponse.StatusCode;
        Response.ContentType = "video/mp4";
        using Stream stream = await forwardResponse.Content.ReadAsStreamAsync();
        await stream.CopyToAsync(Response.Body);

        // Broadcast video viewing details via RabbitMQ
        // Sends the "viewed" message to indicate this video has been viewed.
        _rabbit.BroadcastVideoViewedMessage(id);
    }
}