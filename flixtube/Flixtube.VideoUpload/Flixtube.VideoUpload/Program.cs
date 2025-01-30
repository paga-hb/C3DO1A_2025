using Flixtube.VideoUpload.Services;

var builder = WebApplication.CreateBuilder(args);

// Make sure the necessary environment variables are available.
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_UPLOAD_PORT"))) {
    throw new Exception("Please specify the port number for the HTTP server with the environment variable FLIXTUBE_VIDEO_UPLOAD_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_SCHEME"))) {
    throw new Exception("Please specify the scheme for Flixtube.VideoStorage with the environment variable FLIXTUBE_VIDEO_STORAGE_SCHEME.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_HOST"))) {
    throw new Exception("Please specify the host for Flixtube.VideoStorage with the environment variable FLIXTUBE_VIDEO_STORAGE_HOST.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.VideoStorage with the environment variable FLIXTUBE_VIDEO_STORAGE_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_RABBIT_MQ_HOST"))) {
    throw new Exception("Please specify the host for RabbitMQ with the environment variable FLIXTUBE_RABBIT_MQ_HOST.");
}

// Get necessary environment variables
// Note that we only need to get environment variables here if there are need before builder.Build()
// int VIDEO_UPLOAD_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_UPLOAD_PORT") ?? "80");
// string VIDEO_STORAGE_SCHEME = Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_SCHEME") ?? string.Empty;
// string VIDEO_STORAGE_HOST = Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_HOST") ?? string.Empty;
// int VIDEO_STORAGE_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_VIDEO_STORAGE_PORT") ?? "80");
// string RABBIT_MQ_HOST = Environment.GetEnvironmentVariable("FLIXTUBE_RABBIT_MQ_HOST") ?? string.Empty;

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

builder.Services.AddScoped<IRabbitMqPublisherService, RabbitMqPublisherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

int VIDEO_UPLOAD_PORT = app.Configuration.GetValue<int>("VIDEO_UPLOAD_PORT");
Console.WriteLine($"Microservice online and listening on port {VIDEO_UPLOAD_PORT}.");
app.Run($"http://0.0.0.0:{VIDEO_UPLOAD_PORT}");