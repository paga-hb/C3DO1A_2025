using System.Net;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
// https://learn.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet

namespace Flixtube.AzureStorage.Controllers;

[ApiController]
[Route("/")]
public class AzureStorageController : ControllerBase
{
    private readonly ILogger<AzureStorageController> _logger;
    private readonly IConfiguration _config;
    private readonly string STORAGE_ACCOUNT_NAME;
    private readonly string STORAGE_ACCESS_KEY;
    private readonly string STORAGE_CONTAINER_NAME;

    public AzureStorageController(ILogger<AzureStorageController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;

        STORAGE_ACCOUNT_NAME = _config.GetValue<string>("STORAGE_ACCOUNT_NAME")!;
        STORAGE_ACCESS_KEY = _config.GetValue<string>("STORAGE_ACCESS_KEY")!;
        STORAGE_CONTAINER_NAME = _config.GetValue<string>("STORAGE_CONTAINER_NAME")!;

        _logger.LogInformation("AzureStorageController() called.");
    }

    // Create the Blob service API to communicate with Azure storage.
    private BlobServiceClient CreateBlobService()
    {
        //var sharedKeyCredential = new DefaultAzureCredential();
        var sharedKeyCredential = new StorageSharedKeyCredential(STORAGE_ACCOUNT_NAME, STORAGE_ACCESS_KEY);
        var blobService = new BlobServiceClient
        (
            new Uri($"https://{STORAGE_ACCOUNT_NAME}.blob.core.windows.net"),
            sharedKeyCredential
        );
        return blobService;
    }

    // Health check.
    [HttpGet("/health")]
    public async Task<IActionResult> Health()
    {
        await Task.Delay(0);
        return Ok();
    }

    // Stream video from the Azure Blob Container to the video streaming microservice.
    [HttpGet("/video")]
    public async Task StreamVideo()
    {
        _logger.LogInformation($"StreamVideo() called.");

        var videoId = Request.Query["id"];
        _logger.LogInformation($"Video {videoId}");
        
        // Get video from the Azure Blob Container.
        BlobServiceClient blobService = CreateBlobService();
        BlobContainerClient containerClient = blobService.GetBlobContainerClient(STORAGE_CONTAINER_NAME);
        BlobClient blobClient = containerClient.GetBlobClient(videoId);
        var properties = await blobClient.GetPropertiesAsync();

        // Write HTTP headers to the response.
        Response.StatusCode = (int)HttpStatusCode.OK;
        Response.ContentLength = properties.Value.ContentLength;
        Response.ContentType = "video/mp4";

        // Stream video from the Azure Blob Container to the HTTP response stream.
        await blobClient.DownloadToAsync(Response.Body);
    }

    // Upload video to the Azure Blob Container from the video upload microservice.
    [HttpPost("/video")]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        _logger.LogInformation($"UploadVideo() called.");

        // var contentType = Request.Headers.ContentType.ToString();
        // var videoId = Request.Query["id"];
        // _logger.LogInformation($"Video {videoId}");

        if (file == null || file.Length == 0)
            return BadRequest("File is missing.");
        
        var fileName = file.FileName;
        var fileSize = file.Length;
        var videoId = Request.Headers["id"];
        var contentType = Request.Headers.ContentType.ToString();
        
        // _logger.LogInformation($"fileName: {fileName}");
        // _logger.LogInformation($"fileSize: {fileSize}");
        // _logger.LogInformation($"videoId: {videoId}");
        // _logger.LogInformation($"contentType: {contentType}");

        // Create Azure Blob Container if it doesn't exist.
        BlobServiceClient blobService = CreateBlobService();
        BlobContainerClient containerClient = blobService.GetBlobContainerClient(STORAGE_CONTAINER_NAME);
        await containerClient.CreateIfNotExistsAsync();
        
        // Upload video to the Azure Blob Container.
        BlobClient blobClient = containerClient.GetBlobClient(videoId);
        // var properties = await blobClient.GetPropertiesAsync();
        using var stream = file.OpenReadStream();
        // await blobClient.UploadAsync(Request.Body);
        await blobClient.UploadAsync(stream);
        await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = contentType });

        // // Write HTTP headers to the response.
        // Response.StatusCode = (int)HttpStatusCode.OK;

        return Ok(new
        {
            Message = "File uploaded successfully",
            FileName = fileName,
            FileSize = fileSize
        });
    }

    // Delete a video from the Azure Blob Container.
    [HttpDelete("/video/{id}")]
    public async Task<IActionResult> DeleteVideo(string id)
    {
        _logger.LogInformation($"DeleteVideo() called.");

        var contentType = Request.Headers.ContentType.ToString();
        // var videoId = Request.Query["id"];
        var videoId = id;
        _logger.LogInformation($"Video {videoId}");

        // Get video from the Azure Blob Container.
        BlobServiceClient blobService = CreateBlobService();
        BlobContainerClient containerClient = blobService.GetBlobContainerClient(STORAGE_CONTAINER_NAME);
        BlobClient blobClient = containerClient.GetBlobClient(videoId);
        await containerClient.CreateIfNotExistsAsync();
        // var properties = await blobClient.GetPropertiesAsync();
        
        var exists = await blobClient.ExistsAsync();
        if(!exists)
        {
            // Response.StatusCode = (int)HttpStatusCode.NotFound;
            // return;
            return NotFound();
        }

        // Delete the video from the Blob Container.
        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        
        // Response.StatusCode = (int)HttpStatusCode.OK;

        return Ok(new
        {
            Message = "File deleted successfully",
            FileName = id
        });
    }
}