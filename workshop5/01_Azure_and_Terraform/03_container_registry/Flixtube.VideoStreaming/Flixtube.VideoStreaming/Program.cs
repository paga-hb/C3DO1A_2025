using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Make sure the necessary environment variables are available.

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STREAMING_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.VideoStreaming with the environment variable FLIXTUBE_VIDEO_STREAMING_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_STORAGE_FOLDER_NAME"))) {
    throw new Exception("Please specify the Filesystem folder name for Flixtube.VideoStreaming with the subkey FLIXTUBE_STORAGE_FOLDER_NAME.");
}

// Get necessary environment variables
// Note that we only need to get settings here if there are need before builder.Build()
// int VIDEO_STREAMING_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STREAMING_PORT") ?? "80");
// string STORAGE_FOLDER_NAME = Environment.GetEnvironmentVariable("FLIXTUBE_STORAGE_FOLDER_NAME") ?? string.Empty;

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

int VIDEO_STREAMING_PORT = app.Configuration.GetValue<int>("VIDEO_STREAMING_PORT");
Console.WriteLine($"Microservice online and listening on port {VIDEO_STREAMING_PORT}.");
app.Run($"http://0.0.0.0:{VIDEO_STREAMING_PORT}");