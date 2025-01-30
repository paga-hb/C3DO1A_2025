using System.Net.Http.Headers;
using Flixtube.Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace Flixtube.Gateway.Controllers;

[ApiController]
[Route("api")]
public class GatewayController : ControllerBase
{
    private readonly ILogger<GatewayController> _logger;
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public GatewayController(ILogger<GatewayController> logger, IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _config = config;
        _httpClientFactory = httpClientFactory;
        
        _logger.LogInformation("GatewayController() called.");
    }

    // Health check.
    [HttpGet("/health")]
    public async Task<IActionResult> Health()
    {
        await Task.Delay(0);
        return Ok();
    }

    // Get all metadata from the Metadata microservice.
    [HttpGet("metadata")]
    public async Task<IActionResult> GetMetadata()
    {
        _logger.LogInformation("GetMetadata() called.");
        var client = _httpClientFactory.CreateClient("MetadataClient");
        var content = await client.GetFromJsonAsync<List<Video>>("/videos") ?? [];
        return Ok(content);
    }

    // Get metadata for a specific video from the Metadata microservice.
    [HttpGet("metadata/{id}")]
    public async Task<IActionResult> GetMetadata(string id)
    {
        _logger.LogInformation($"GetMetadata({id}) called.");
        var client = _httpClientFactory.CreateClient("MetadataClient");
        var content = await client.GetFromJsonAsync<Video>($"/video/{id}");
        if(content == null)
        {
            return NotFound();
        }
        return Ok(content);
    }

    // Add metadata for a specific video to the Metadata microservice.
    [HttpPost("metadata")]
    public async Task<IActionResult> AddMetadata([FromBody] Video video)
    {
        _logger.LogInformation($"AddMetadata({video}) called.");
        var client = _httpClientFactory.CreateClient("MetadataClient");
        var content = await client.PostAsJsonAsync<Video>($"/video", video);
        if(content == null)
        {
            return NotFound();
        }
        return Ok(content);
    }

    // Delete metadata for a specific video from the Metadata microservice.
    [HttpDelete("metadata/{id}")]
    public async Task<IActionResult> DeleteMetadata(string id)
    {
        _logger.LogInformation($"DeleteMetadata({id}) called.");
        var client = _httpClientFactory.CreateClient("MetadataClient");
        var response = await client.DeleteAsync($"/video/{id}");
        if(!response.IsSuccessStatusCode)
        {
            return NotFound();
        }
        return Ok();
    }

    // Get all all video viewing history from the History microservice.
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        _logger.LogInformation($"GetHistory() called.");
        var client = _httpClientFactory.CreateClient("HistoryClient");
        var content = await client.GetFromJsonAsync<List<ViewHistory>>("/history") ?? [];
        return Ok(content);
    }

    // Stream video from the video streaming microservice to the Flixtube.Web app in the user's browser.
    [HttpGet("video/{id}")]
    public async Task StreamVideo(string id)
    {
        _logger.LogInformation($"StreamVideo({id}) called.");

        var client = _httpClientFactory.CreateClient("VideoStreamingClient");

        // Forward request to the video streaming microservice.
        HttpRequestMessage forwardRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{client.BaseAddress.ToString()}video/{id}")
        };
        Request.Headers.ToList().ForEach(header => forwardRequest.Headers.Add(header.Key, header.Value.ToString()));
        HttpResponseMessage forwardResponse = await client.SendAsync(forwardRequest);
        // _logger.LogInformation($"Success: {forwardResponse.IsSuccessStatusCode}");
        
        // Forward response from the video streaming microservice.
        forwardResponse.Headers.ToList().ForEach(header => Response.Headers.Append(header.Key, header.Value.ToString()));
        Response.StatusCode = (int)forwardResponse.StatusCode;
        Response.ContentType = "video/mp4";
        using Stream stream = await forwardResponse.Content.ReadAsStreamAsync();
        await stream.CopyToAsync(Response.Body);
    }

    // Upload video from the Flixtube.Web app to the video upload microservice.
    [HttpPost("video")]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        _logger.LogInformation($"UploadVideo({file}) called.");
        
        var client = _httpClientFactory.CreateClient("VideoUploadClient");

        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        // Prepare multipart form-data content
        var content = new MultipartFormDataContent();
        await using var fileStream = file.OpenReadStream();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
        content.Add(fileContent, "file", file.FileName);

        // Forward request to the video upload microservice
        var targetUrl = new Uri($"{client.BaseAddress.ToString()}video");
        var response = await client.PostAsync(targetUrl, content);
        // _logger.LogInformation($"Success: {response.IsSuccessStatusCode}");

        // Forward response from the video upload microservice
        var responseBody = await response.Content.ReadAsStringAsync();
        return new ContentResult
        {
            StatusCode = (int)response.StatusCode,
            Content = responseBody,
            ContentType = response.Content.Headers.ContentType?.ToString()
        };
    }

    // Delete a video from the video storage microservice.
    [HttpDelete("video/{id}")]
    public async Task<IActionResult> DeleteVideo(string id)
    {
        _logger.LogInformation($"DeleteVideo({id}) called.");
        var client = _httpClientFactory.CreateClient("VideoStorageClient");
        var response = await client.DeleteAsync($"/video/{id}");
        if(!response.IsSuccessStatusCode)
        {
            return NotFound();
        }
        return Ok();
    }
}