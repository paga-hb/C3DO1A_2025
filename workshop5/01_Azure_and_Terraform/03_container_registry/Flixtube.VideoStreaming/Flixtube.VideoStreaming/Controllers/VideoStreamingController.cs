using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Flixtube.VideoStreaming.Controllers;

[ApiController]
[Route("/")]
public class VideoStreamingController : ControllerBase
{
    private readonly ILogger<VideoStreamingController> _logger;
    private readonly IConfiguration _config;
    private readonly string STORAGE_FOLDER_NAME;

    public VideoStreamingController(ILogger<VideoStreamingController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;

        STORAGE_FOLDER_NAME = _config.GetValue<string>("STORAGE_FOLDER_NAME")!;

        _logger.LogInformation("VideoStreamingController() called.");
    }

    // Health check.
    [HttpGet("/health")]
    public async Task<IActionResult> Health()
    {
        await Task.Delay(0);
        return Ok();
    }

    // Stream video from the Filesystem.
    [HttpGet("{id}")]
    public async Task StreamVideo(string id)
    {
        // _logger.LogInformation($"StreamVideo() called.");
        _logger.LogInformation($"StreamVideo({id}) called.");

        // Get video from the Filesystem.
        var videoPath = $"./{STORAGE_FOLDER_NAME}/{id}";
        if(!System.IO.File.Exists(videoPath))
        {
            Response.StatusCode = StatusCodes.Status404NotFound; return;
        }
        var size = (new FileInfo(videoPath)).Length;

        // Write HTTP headers to the response.
        Response.StatusCode = (int)HttpStatusCode.OK;
        Response.ContentLength = size;
        Response.ContentType = "video/mp4";

        // Stream video from the Filesystem to the HTTP response stream.
        using var stream = new FileStream(videoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        await stream.CopyToAsync(Response.Body, (int)size);
    }

    // Upload video to the Filesystem.
    [HttpPost()]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        _logger.LogInformation($"UploadVideo() called.");

        if (file == null || file.Length == 0)
            return BadRequest("File is missing.");
        
        var fileName = file.FileName;
        var fileSize = file.Length;
        var videoId = Guid.NewGuid().ToString();
        
        // _logger.LogInformation($"fileName: {fileName}");
        // _logger.LogInformation($"fileSize: {fileSize}");
        // _logger.LogInformation($"videoId: {videoId}");

        // Create Filesystem path to store the video.
        var videoPath = $"./{STORAGE_FOLDER_NAME}/{videoId}.mp4";

        // Ensure the Filesystem folder exists.
        var directory = Path.GetDirectoryName(videoPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory ?? string.Empty);
        }

        // Store the video to the Filesystem.
        using (var fileStream = new FileStream(videoPath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return Ok(new
        {
            Message = "File uploaded successfully",
            FileName = fileName,
            FileSize = fileSize,
            VideoId = videoId
        });
    }

    // Delete a video from the Filesystem.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVideo(string id)
    {
        _logger.LogInformation($"DeleteVideo() called.");

        await Task.Delay(0);

        var contentType = Request.Headers.ContentType.ToString();
        _logger.LogInformation($"Video {id}");

        // Get video from the Filesystem.
        var videoPath = $"./{STORAGE_FOLDER_NAME}/{id}.mp4";
        if (!System.IO.File.Exists(videoPath))
        {
            return NotFound();
        }

        // Delete the video from the Filesystem.
        System.IO.File.Delete(videoPath);
        
        return Ok(new
        {
            Message = "File deleted successfully",
            FileName = id
        });
    }
}