using System.Net;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
// https://min.io/docs/minio/linux/developers/dotnet/minio-dotnet.html
// https://sanidhya235.medium.com/introduction-to-minio-193e8523a4a8
// https://www.youtube.com/@MINIO/search?query=%23dotnet
// Access Keys -> Create access key

namespace Flixtube.MinioStorage.Controllers;

[ApiController]
[Route("/")]
public class MinioStorageController : ControllerBase
{
    private readonly ILogger<MinioStorageController> _logger;
    private readonly IConfiguration _config;
    private readonly string STORAGE_ENDPOINT;
    private readonly string STORAGE_ACCESS_KEY;
    private readonly string STORAGE_SECRET_KEY;
    private readonly string STORAGE_BUCKET_NAME;

    public MinioStorageController(ILogger<MinioStorageController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;

        STORAGE_ENDPOINT = _config.GetValue<string>("STORAGE_ENDPOINT") ?? string.Empty;
        STORAGE_ACCESS_KEY = _config.GetValue<string>("STORAGE_ACCESS_KEY") ?? string.Empty;
        STORAGE_SECRET_KEY = _config.GetValue<string>("STORAGE_SECRET_KEY") ?? string.Empty;
        STORAGE_BUCKET_NAME = _config.GetValue<string>("STORAGE_BUCKET_NAME") ?? string.Empty;

        _logger.LogInformation("MinioStorageController() called.");
    }

    // Creates and returns the Minio client to communicate with Minio storage.
    private IMinioClient GetMinioClient()
    {
        IMinioClient minioClient = new MinioClient()
            .WithEndpoint(STORAGE_ENDPOINT)
            .WithCredentials(STORAGE_ACCESS_KEY, STORAGE_SECRET_KEY)
            .WithSSL(false)
            .Build();
        return minioClient;
    }

    // Health check.
    [HttpGet("/health")]
    public async Task<IActionResult> Health()
    {
        await Task.Delay(0);
        return Ok();
    }

    // Stream video from the Minio bucket to the video streaming microservice.
    [HttpGet("/video")]
    public async Task StreamVideo() // Get(IMinioClient minioClient)
    {
        _logger.LogInformation($"StreamVideo() called.");

        var req = HttpContext.Request;
        var res = HttpContext.Response;
        
        var objectName = req.Query["id"];
        _logger.LogInformation($"objectName: {objectName}.");
        
        // Create the Minio client to communicate with Minio storage.
        IMinioClient minioClient = GetMinioClient();

        // Make sure the Minio bucket exists.
        BucketExistsArgs bucketExistsArgs = new BucketExistsArgs().WithBucket(STORAGE_BUCKET_NAME);
        var found = await minioClient.BucketExistsAsync(bucketExistsArgs);
        if(!found)
        {
            // Create the "videos" bucket.
            MakeBucketArgs makeBucketArgs = new MakeBucketArgs().WithBucket(STORAGE_BUCKET_NAME);
            await minioClient.MakeBucketAsync(makeBucketArgs);

            // Set the Minio bucket policy for "videos" to allow public access.
            string publicPolicy = @"
            {
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {
                        ""Effect"": ""Allow"",
                        ""Principal"": ""*"",
                        ""Action"": ""s3:*"",
                        ""Resource"": ""arn:aws:s3:::STORAGE_BUCKET_NAME/*""
                    }
                ]
            }
            ";
            publicPolicy.Replace("STORAGE_BUCKET_NAME", STORAGE_BUCKET_NAME);
            SetPolicyArgs setPolicyArgs = new SetPolicyArgs().WithPolicy(publicPolicy).WithBucket(STORAGE_BUCKET_NAME);
            await minioClient.SetPolicyAsync(setPolicyArgs);
        }
        // _logger.LogInformation($"Found: {found}.");

        // Make sure the object (video) exists in the bucket.
        StatObjectArgs statObjectArgs = new StatObjectArgs().WithBucket(STORAGE_BUCKET_NAME).WithObject(objectName);
        var objectStat = await minioClient.StatObjectAsync(statObjectArgs); // throws Minio.Exceptions.ObjectNotFoundException
        // _logger.LogInformation($"bucketName: {STORAGE_BUCKET_NAME}");
        // _logger.LogInformation($"objectName: {objectName}");
        // _logger.LogInformation($"Object Stat: {objectStat}");

        // Write HTTP headers to the response.
        res.StatusCode = (int)HttpStatusCode.OK;
        res.ContentLength = objectStat.Size;
        res.ContentType = "video/mp4";

        // Fetch the video object from the Minio bucket and write it to the response body.
        using var memoryStream = new MemoryStream();
        GetObjectArgs getObjectArgs = new GetObjectArgs()
            .WithBucket(STORAGE_BUCKET_NAME)
            .WithObject(objectName)
            //    .WithOffsetAndLength(0L, objectStat.Size)
            .WithCallbackStream( (stream) => { stream.CopyTo(memoryStream); });
        await minioClient.GetObjectAsync(getObjectArgs);
        // _logger.LogInformation($"memoryStream.Length: {memoryStream.Length}");
        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(res.Body);
    }

    // Upload video to the Minio bucket from the video upload microservice.
    [HttpPost("/video")]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        _logger.LogInformation($"UploadVideo({file}) called.");

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

        // Create the Minio client to communicate with Minio storage. 
        IMinioClient minioClient = GetMinioClient();

        // Create the Minio bucket if it doesn't exist.
        BucketExistsArgs bucketExistsArgs = new BucketExistsArgs().WithBucket(STORAGE_BUCKET_NAME);
        var found = await minioClient.BucketExistsAsync(bucketExistsArgs);
        if(!found)
        {
            // Create the "videos" bucket.
            MakeBucketArgs makeBucketArgs = new MakeBucketArgs().WithBucket(STORAGE_BUCKET_NAME);
            await minioClient.MakeBucketAsync(makeBucketArgs);

            // Set the Minio bucket policy for "videos" to allow public access.
            string publicPolicy = @"
            {
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {
                        ""Effect"": ""Allow"",
                        ""Principal"": ""*"",
                        ""Action"": ""s3:*"",
                        ""Resource"": ""arn:aws:s3:::STORAGE_BUCKET_NAME/*""
                    }
                ]
            }
            ";
            publicPolicy.Replace("STORAGE_BUCKET_NAME", STORAGE_BUCKET_NAME);
            SetPolicyArgs setPolicyArgs = new SetPolicyArgs().WithPolicy(publicPolicy).WithBucket(STORAGE_BUCKET_NAME);
            await minioClient.SetPolicyAsync(setPolicyArgs);
        }
        // _logger.LogInformation($"Found: {found}.");
        
        // Upload the video to the Minio bucket.
        using var stream = file.OpenReadStream();
        await minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(STORAGE_BUCKET_NAME)
            // .WithObject(fileName)
            .WithObject(videoId)
            .WithStreamData(stream)
            .WithObjectSize(file.Length)
            .WithContentType(contentType));
            // .WithContentType("video/mp4"));

        return Ok(new
        {
            Message = "File uploaded successfully",
            FileName = fileName,
            FileSize = fileSize
        });
    }

    // Delete a video from the Minio bucket.
    [HttpDelete("/video/{id}")]
    public async Task<IActionResult> DeleteVideo(string id)
    {
        _logger.LogInformation($"DeleteVideo({id}) called.");

        // Create the Minio client to communicate with Minio storage.
        IMinioClient minioClient = GetMinioClient();

        // Make sure the Minio bucket exists.
        BucketExistsArgs bucketExistsArgs = new BucketExistsArgs().WithBucket(STORAGE_BUCKET_NAME);
        var found = await minioClient.BucketExistsAsync(bucketExistsArgs);
        if(!found)
        {
            // Create the "videos" bucket.
            MakeBucketArgs makeBucketArgs = new MakeBucketArgs().WithBucket(STORAGE_BUCKET_NAME);
            await minioClient.MakeBucketAsync(makeBucketArgs);

            // Set the Minio bucket policy for "videos" to allow public access.
            string publicPolicy = @"
            {
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {
                        ""Effect"": ""Allow"",
                        ""Principal"": ""*"",
                        ""Action"": ""s3:*"",
                        ""Resource"": ""arn:aws:s3:::STORAGE_BUCKET_NAME/*""
                    }
                ]
            }
            ";
            publicPolicy.Replace("STORAGE_BUCKET_NAME", STORAGE_BUCKET_NAME);
            SetPolicyArgs setPolicyArgs = new SetPolicyArgs().WithPolicy(publicPolicy).WithBucket(STORAGE_BUCKET_NAME);
            await minioClient.SetPolicyAsync(setPolicyArgs);
        }
        
        // Delete the video from the Minio bucket.
        await minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(STORAGE_BUCKET_NAME)
                .WithObject(id));

        return Ok(new
        {
            Message = "File deleted successfully",
            FileName = id
        });
    }
}