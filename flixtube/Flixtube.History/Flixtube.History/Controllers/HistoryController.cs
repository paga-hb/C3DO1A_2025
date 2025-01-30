using Microsoft.AspNetCore.Mvc;
using Flixtube.History.Repositories;

namespace Flixtube.History.Controllers;

[ApiController]
[Route("/")]
public class HistoryController : ControllerBase
{
    private readonly ILogger<HistoryController> _logger;
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;

    public HistoryController(ILogger<HistoryController> logger, IConfiguration config, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _config = config;
        _unitOfWork = unitOfWork;

        _logger.LogInformation("HistoryController() called.");
    }

    // Health check.
    [HttpGet("/health")]
    public async Task<IActionResult> Health()
    {
        await Task.Delay(0);
        return Ok();
    }

    // Get viewing history for all videos.
    [HttpGet("/history")]
    public async Task<IActionResult> GetAllHistory()
    {
        _logger.LogInformation("GetAllHistory() called.");
        var history = await _unitOfWork.ViewHistorys.FindAsync();
        return Ok(history);
    }
}