var builder = WebApplication.CreateBuilder(args);

// Make sure the necessary environment variables are available.
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_AZURE_STORAGE_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.AzureStorage with the environment variable FLIXTUBE_AZURE_STORAGE_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_STORAGE_ACCOUNT_NAME"))) {
    throw new Exception("Please specify the name of an Azure storage account in environment variable FLIXTUBE_STORAGE_ACCOUNT_NAME.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_STORAGE_ACCESS_KEY"))) {
    throw new Exception("Please specify the access key to an Azure storage account in environment variable FLIXTUBE_STORAGE_ACCESS_KEY.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_STORAGE_CONTAINER_NAME"))) {
    throw new Exception("Please specify the name of an Azure storage container in environment variable FLIXTUBE_STORAGE_CONTAINER_NAME.");
}

// Get necessary environment variables
// Note that we only need to get environment variables here if there are need before builder.Build()
// int AZURE_STORAGE_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_AZURE_STORAGE_PORT") ?? "80");
// string STORAGE_ACCOUNT_NAME = Environment.GetEnvironmentVariable("FLIXTUBE_STORAGE_ACCOUNT_NAME") ?? string.Empty;
// string STORAGE_ACCESS_KEY = Environment.GetEnvironmentVariable("FLIXTUBE_STORAGE_ACCESS_KEY") ?? string.Empty;
// string STORAGE_CONTAINER_NAME = Environment.GetEnvironmentVariable("FLIXTUBE_STORAGE_CONTAINER_NAME") ?? string.Empty;

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
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

int AZURE_STORAGE_PORT = app.Configuration.GetValue<int>("AZURE_STORAGE_PORT");
Console.WriteLine($"Microservice online and listening on port {AZURE_STORAGE_PORT}.");
app.Run($"http://0.0.0.0:{AZURE_STORAGE_PORT}");