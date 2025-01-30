using Microsoft.EntityFrameworkCore;
using Flixtube.Metadata.Data;
using Flixtube.Metadata.Repositories;
using Flixtube.Metadata.Services;

var builder = WebApplication.CreateBuilder(args);

// Make sure the necessary environment variables are available.
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.Metadata with the environment variable FLIXTUBE_METADATA_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_CONNECTION_STRING"))) {
    throw new Exception("Please specify the database conenction string using environment variable FLIXTUBE_METADATA_CONNECTION_STRING.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_SEED_DATABASE"))) {
    throw new Exception("Please specify if the database should be seeded using environment variable FLIXTUBE_METADATA_SEED_DATABASE.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_RABBIT_MQ_HOST"))) {
    throw new Exception("Please specify the host for RabbitMQ with the environment variable FLIXTUBE_RABBIT_MQ_HOST.");
}

// Get necessary environment variables
// Note that we only need to get environment variables here if there are need before builder.Build()
// int METADATA_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_PORT") ?? "80");
string METADATA_CONNECTION_STRING = Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_CONNECTION_STRING") ?? string.Empty;
bool METADATA_SEED_DATABASE = bool.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_METADATA_SEED_DATABASE") ?? "false");
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

builder.Services.AddHostedService<RabbitMqSubscriberService>(); // Register RabbitMQSubscriber as a background service

builder.Services.AddDbContext<ApplicationDbContext>(
    // options => options.UseSqlServer(builder.Configuration.GetConnectionString("video_streaming"))
    options => options.UseSqlServer(METADATA_CONNECTION_STRING)
);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // var dbContext = services.GetRequiredService<ApplicationDbContext>();
    // var connection = dbContext.Database.GetDbConnection();
    // while (true)
    // {
    //     try
    //     {
    //         if (connection.State != System.Data.ConnectionState.Open)
    //             await connection.OpenAsync();
    //         // Connection is successful
    //         break;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Database connection attempt failed: {ex.Message}");
    //         await Task.Delay(5000); // Wait before retrying
    //     }
    // }
    await SeedData.Initialize(services, METADATA_SEED_DATABASE);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

int METADATA_PORT = app.Configuration.GetValue<int>("METADATA_PORT");
Console.WriteLine($"Microservice online and listening on port {METADATA_PORT}.");
app.Run($"http://0.0.0.0:{METADATA_PORT}");