using Microsoft.AspNetCore.Mvc;
using Flixtube.Metadata.Repositories;
using Flixtube.Metadata.Entities;

namespace Flixtube.Metadata.Controllers;

[ApiController]
[Route("/")]
public class MetadataController : ControllerBase
{
    private readonly ILogger<MetadataController> _logger;
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;

    public MetadataController(ILogger<MetadataController> logger, IConfiguration config, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _config = config;
        _unitOfWork = unitOfWork;

        _logger.LogInformation("MetadataController() called.");
    }

    // Health check.
    [HttpGet("/health")]
    public async Task<IActionResult> Health()
    {
        await Task.Delay(0);
        return Ok();
    }

    // Get metadata for all videos.
    [HttpGet("/videos")]
    public async Task<IActionResult> GetVideos()
    {
        _logger.LogInformation("GetVideos() called.");
        var videos = await _unitOfWork.Videos.FindAsync();
        return Ok(videos);
    }

    // Get metadata for a specific video.
    [HttpGet("/video/{id}")]
    public async Task<IActionResult> GetVideo(string id)
    {
        _logger.LogInformation($"GetVideo({id}) called.");

        var video = await _unitOfWork.Videos.FirstOrDefaultAsync(v => v.Id == id);

        if (video == null) // The video was not found
        {
            _logger.LogInformation($"Video {id} not found.");
            return NotFound();
        }

        return Ok(video);
    }

    // Add metadata for a specific video.
    [HttpPost("/video")]
    public async Task<IActionResult> AddVideo([FromBody] Video video)
    {
        _logger.LogInformation($"AddVideo({video}) called.");

        var existingVideo = await _unitOfWork.Videos.FirstOrDefaultAsync(v => v.Id == video.Id);

        if (existingVideo != null)
        {
            return BadRequest("Video already exists.");
        }

        _unitOfWork.Videos.Add(video);
        await _unitOfWork.CompleteAsync();

        return Ok(video);
    }

    // Delete metadata for a specific video.
    [HttpDelete("/video/{id}")]
    public async Task<IActionResult> DeleteVideo(string id)
    {
        _logger.LogInformation($"DeleteVideo({id}) called.");

        var video = await _unitOfWork.Videos.FirstOrDefaultAsync(v => v.Id == id);

        if (video == null) // The video was not found
        {
            return NotFound();
        }

        _unitOfWork.Videos.Remove(video);
        await _unitOfWork.CompleteAsync();

        return Ok(video);
    }
}