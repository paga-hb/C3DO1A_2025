using Microsoft.AspNetCore.Mvc;
using Flixtube.Data.Repositories;

namespace Flixtube.Gateway.Controllers;

[ApiController]
[Route("/api/[controller]")]
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

    // Get viewing history for all videos.
    [HttpGet()]
    public async Task<IActionResult> GetAllHistory()
    {
        _logger.LogInformation("GetAllHistory() called.");
        var history = await _unitOfWork.ViewHistorys.FindAsync();
        return Ok(history);
    }
}