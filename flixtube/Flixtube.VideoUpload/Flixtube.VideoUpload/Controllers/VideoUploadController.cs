using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Flixtube.VideoUpload.Services;

namespace Flixtube.VideoUpload.Controllers;

[ApiController]
[Route("/")]
public class VideoUploadController : ControllerBase
{
    private readonly ILogger<VideoUploadController> _logger;
    private readonly IConfiguration _config;
    private readonly IRabbitMqPublisherService _rabbit;
    private readonly string VIDEO_STORAGE_SCHEME;
    private readonly string VIDEO_STORAGE_HOST;
    private readonly int VIDEO_STORAGE_PORT;

    public VideoUploadController(ILogger<VideoUploadController> logger, IConfiguration config, IRabbitMqPublisherService rabbit)
    {
        _logger = logger;
        _config = config;
        _rabbit = rabbit;

        VIDEO_STORAGE_SCHEME = _config.GetValue<string>("VIDEO_STORAGE_SCHEME")!;
        VIDEO_STORAGE_HOST = _config.GetValue<string>("VIDEO_STORAGE_HOST")!;
        VIDEO_STORAGE_PORT = _config.GetValue<int>("VIDEO_STORAGE_PORT");

        _logger.LogInformation("VideoUploadController() called.");
    }

    // Health check.
    [HttpGet("/health")]
    public async Task<IActionResult> Health()
    {
        await Task.Delay(0);
        return Ok();
    }

    // Upload video from the Flixtube.Gateway to the video upload microservice.
    [HttpPost("/video")]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        _logger.LogInformation($"UploadVideo({file}) called.");

        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var fileName = file.FileName;
        var videoId = Guid.NewGuid().ToString();

        // Prepare multipart form-data content
        var content = new MultipartFormDataContent();
        await using var fileStream = file.OpenReadStream();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
        content.Add(fileContent, "file", file.FileName);
        content.Headers.Add("id", videoId);

        // Forward request to the video storage microservice
        using HttpClient httpClient = new();
        var targetUrl = new Uri($"{VIDEO_STORAGE_SCHEME}://{VIDEO_STORAGE_HOST}:{VIDEO_STORAGE_PORT}/video");
        var response = await httpClient.PostAsync(targetUrl, content);
        // _logger.LogInformation($"Success: {response.IsSuccessStatusCode}");

        // Broadcast new video details via RabbitMQ
        // Sends the "uploaded" message to indicate this video has been uploaded.
        _rabbit.BroadcastVideoUploadedMessage(videoId, fileName);

        // Forward response from the video storage microservice
        var responseBody = await response.Content.ReadAsStringAsync();
        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = responseBody,
            ContentType = response.Content.Headers.ContentType?.ToString()
        };
    }
}