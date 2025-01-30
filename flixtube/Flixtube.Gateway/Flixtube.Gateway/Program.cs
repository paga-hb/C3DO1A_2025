using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Make sure the necessary environment variables are available.
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_GATEWAY_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.Gateway with the environment variable FLIXTUBE_GATEWAY_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_SCHEME"))) {
    throw new Exception("Please specify the scheme for Flixtube.Metadata with the environment variable FLIXTUBE_METADATA_SCHEME.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_HOST"))) {
    throw new Exception("Please specify the host for Flixtube.Metadata with the environment variable FLIXTUBE_METADATA_HOST.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.Metadata with the environment variable FLIXTUBE_METADATA_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_HISTORY_SCHEME"))) {
    throw new Exception("Please specify the scheme for Flixtube.History with the environment variable FLIXTUBE_HISTORY_SCHEME.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_HISTORY_HOST"))) {
    throw new Exception("Please specify the host for Flixtube.History with the environment variable FLIXTUBE_HISTORY_HOST.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_HISTORY_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.History with the environment variable FLIXTUBE_HISTORY_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STREAMING_SCHEME"))) {
    throw new Exception("Please specify the scheme for Flixtube.VideoStreaming with the environment variable FLIXTUBE_VIDEO_STREAMING_SCHEME.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STREAMING_HOST"))) {
    throw new Exception("Please specify the host for Flixtube.VideoStreaming with the environment variable FLIXTUBE_VIDEO_STREAMING_HOST.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STREAMING_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.VideoStreaming with the environment variable FLIXTUBE_VIDEO_STREAMING_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_UPLOAD_SCHEME"))) {
    throw new Exception("Please specify the scheme for Flixtube.VideoUpload with the environment variable FLIXTUBE_VIDEO_UPLOAD_SCHEME.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_UPLOAD_HOST"))) {
    throw new Exception("Please specify the host for Flixtube.VideoUpload with the environment variable FLIXTUBE_VIDEO_UPLOAD_HOST.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_UPLOAD_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.VideoUpload with the environment variable FLIXTUBE_VIDEO_UPLOAD_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_SCHEME"))) {
    throw new Exception("Please specify scheme number for Flixtube.VideoStorage with the environment variable FLIXTUBE_VIDEO_STORAGE_SCHEME.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_HOST"))) {
    throw new Exception("Please specify the host for Flixtube.VideoStorage with the environment variable FLIXTUBE_VIDEO_STORAGE_HOST.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.VideoStorage with the environment variable FLIXTUBE_VIDEO_STORAGE_PORT.");
}

// Get necessary environment variables
// Note that we only need to get environment variables here if there are need before builder.Build()
// int GATEWAY_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_GATEWAY_PORT") ?? "80");
string METADATA_SCHEME = Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_SCHEME") ?? string.Empty;
string METADATA_HOST = Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_HOST") ?? string.Empty;
int METADATA_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_PORT") ?? "80");
string HISTORY_SCHEME = Environment.GetEnvironmentVariable("FLIXTUBE_HISTORY_SCHEME") ?? string.Empty;
string HISTORY_HOST = Environment.GetEnvironmentVariable("FLIXTUBE_HISTORY_HOST") ?? string.Empty;
int HISTORY_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_HISTORY_PORT") ?? "80");
string VIDEO_STREAMING_SCHEME = Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STREAMING_SCHEME") ?? string.Empty;
string VIDEO_STREAMING_HOST = Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STREAMING_HOST") ?? string.Empty;
int VIDEO_STREAMING_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STREAMING_PORT") ?? "80");
string VIDEO_UPLOAD_SCHEME = Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_UPLOAD_SCHEME") ?? string.Empty;
string VIDEO_UPLOAD_HOST = Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_UPLOAD_HOST") ?? string.Empty;
int VIDEO_UPLOAD_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_UPLOAD_PORT") ?? "80");
string VIDEO_STORAGE_SCHEME = Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_SCHEME") ?? string.Empty;
string VIDEO_STORAGE_HOST = Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_HOST") ?? string.Empty;
int VIDEO_STORAGE_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_PORT") ?? "80");

// Only add environment variables with a "FLIXTUBE_" prefix to the configuration
// Note that the FLIXTUBE_ prefix is removed from the environment variables before adding them to the configuration
// (e.g. FLIXTUBE_GATEWAY_SCHEME becomes GATEWAY_SCHEME).
builder.Configuration.AddEnvironmentVariables("FLIXTUBE_");

// Configure logging.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add HTTP clients for the various microservices to the container.
builder.Services.AddHttpClient("MetadataClient", client => { client.BaseAddress = new Uri($"{METADATA_SCHEME}://{METADATA_HOST}:{METADATA_PORT}"); });
builder.Services.AddHttpClient("HistoryClient", client => { client.BaseAddress = new Uri($"{HISTORY_SCHEME}://{HISTORY_HOST}:{HISTORY_PORT}"); });
builder.Services.AddHttpClient("VideoStreamingClient", client => { client.BaseAddress = new Uri($"{VIDEO_STREAMING_SCHEME}://{VIDEO_STREAMING_HOST}:{VIDEO_STREAMING_PORT}"); });
builder.Services.AddHttpClient("VideoUploadClient", client => { client.BaseAddress = new Uri($"{VIDEO_UPLOAD_SCHEME}://{VIDEO_UPLOAD_HOST}:{VIDEO_UPLOAD_PORT}"); });
builder.Services.AddHttpClient("VideoStorageClient", client => { client.BaseAddress = new Uri($"{VIDEO_STORAGE_SCHEME}://{VIDEO_STORAGE_HOST}:{VIDEO_STORAGE_PORT}"); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(); // scalar/v1
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

int GATEWAY_PORT = app.Configuration.GetValue<int>("GATEWAY_PORT");
Console.WriteLine($"Microservice online and listening on port {GATEWAY_PORT}.");
app.Run($"http://0.0.0.0:{GATEWAY_PORT}");