using System.Net.Http.Headers;
using System.Text.Json;
using Flixtube.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace Flixtube.Web.Services;

public class RestService : IRestService
{
    private readonly ILogger<RestService> _logger;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private JsonSerializerOptions _serializerOptions;
    public string PublicGatewayBaseUrl { get; private set; }

    public RestService(ILogger<RestService> logger, IConfiguration config, HttpClient httpClient)
    {
        _logger = logger;
        _config = config;
        _httpClient = httpClient;
        _serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

        string PUBLIC_GATEWAY_SCHEME = _config.GetValue<string>("PUBLIC_GATEWAY_SCHEME")!;
        string PUBLIC_GATEWAY_HOST = _config.GetValue<string>("PUBLIC_GATEWAY_HOST")!;
        int PUBLIC_GATEWAY_PORT = _config.GetValue<int>("PUBLIC_GATEWAY_PORT")!;
        PublicGatewayBaseUrl = $"{PUBLIC_GATEWAY_SCHEME}://{PUBLIC_GATEWAY_HOST}:{PUBLIC_GATEWAY_PORT}";

        _logger.LogInformation("RestService() called.");
    }

    public async Task<List<Video>> GetMetadataAsync()
    {
        _logger.LogInformation("GetMetadataAsync() called.");
        return await _httpClient.GetFromJsonAsync<List<Video>>("/api/metadata", _serializerOptions) ?? [];
    }

    public async Task<Video> GetMetadataAsync(string id)
    {
        _logger.LogInformation($"GetMetadataAsync({id}) called.");
        return await _httpClient.GetFromJsonAsync<Video>($"/api/metadata/{id}", _serializerOptions) ?? throw new Exception("Not Found");
    }

    public async Task<List<ViewHistory>> GetViewingHistoryAsync()
    {
        _logger.LogInformation("GetViewingHistoryAsync() called.");
        return await _httpClient.GetFromJsonAsync<List<ViewHistory>>("/api/history", _serializerOptions) ?? [];
    }

    public async Task<bool> UploadVideoAsync(IBrowserFile file)
    {
        _logger.LogInformation($"UploadVideoAsync({file} called.");
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream(file.Size));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);
        var response = await _httpClient.PostAsync($"/api/video", content);
        return response.IsSuccessStatusCode;
    }

    public async Task DeleteVideoAsync(string id)
    {
        _logger.LogInformation($"DeleteVideoAsync({id}) called.");
        var videoResult = await _httpClient.DeleteAsync($"/api/video/{id}");
        videoResult.EnsureSuccessStatusCode();
        var metadataResult = await _httpClient.DeleteAsync($"/api/metadata/{id}");
        metadataResult.EnsureSuccessStatusCode();
    }
}