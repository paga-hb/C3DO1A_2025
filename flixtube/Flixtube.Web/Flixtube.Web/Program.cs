using Flixtube.Web.Components;
using Flixtube.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Make sure the necessary environment variables are available.
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_WEB_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.Web with the environment variable FLIXTUBE_WEB_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_GATEWAY_SCHEME"))) {
    throw new Exception("Please specify the scheme for Flixtube.Gateway with the environment variable FLIXTUBE_GATEWAY_SCHEME.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_GATEWAY_HOST"))) {
    throw new Exception("Please specify the host for Flixtube.Gateway with the environment variable FLIXTUBE_GATEWAY_HOST.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_GATEWAY_PORT"))) {
    throw new Exception("Please specify the port number for Flixtube.Gateway with the environment variable FLIXTUBE_GATEWAY_PORT.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_PUBLIC_GATEWAY_SCHEME"))) {
    throw new Exception("Please specify the public scheme for Flixtube.Gateway with the environment variable FLIXTUBE_PUBLIC_GATEWAY_SCHEME.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_PUBLIC_GATEWAY_HOST"))) {
    throw new Exception("Please specify the public host for Flixtube.Gateway with the environment variable FLIXTUBE_PUBLIC_GATEWAY_HOST.");
}

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("FLIXTUBE_PUBLIC_GATEWAY_PORT"))) {
    throw new Exception("Please specify the public port number for Flixtube.Gateway with the environment variable FLIXTUBE_PUBLIC_GATEWAY_PORT.");
}

// Get necessary environment variables
// Note that we only need to get environment variables here if there are need before builder.Build()
// int WEB_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_WEB_PORT") ?? "80");
string GATEWAY_SCHEME = Environment.GetEnvironmentVariable("FLIXTUBE_GATEWAY_SCHEME") ?? string.Empty;
string GATEWAY_HOST = Environment.GetEnvironmentVariable("FLIXTUBE_GATEWAY_HOST") ?? string.Empty;
int GATEWAY_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_GATEWAY_PORT") ?? "80");
// string PUBLIC_GATEWAY_SCHEME = Environment.GetEnvironmentVariable("FLIXTUBE_PUBLIC_GATEWAY_SCHEME") ?? string.Empty;
// string PUBLIC_GATEWAY_HOST = Environment.GetEnvironmentVariable("FLIXTUBE_PUBLIC_GATEWAY_HOST") ?? string.Empty;
// int PUBLIC_GATEWAY_PORT = int.Parse(Environment.GetEnvironmentVariable("FLIXTUBE_PUBLIC_GATEWAY_PORT") ?? "80");

// Only add environment variables with a "FLIXTUBE_" prefix to the configuration
// Note that the FLIXTUBE_ prefix is removed from the environment variables before adding them to the configuration
// (e.g. FLIXTUBE_GATEWAY_SCHEME becomes GATEWAY_SCHEME).
builder.Configuration.AddEnvironmentVariables("FLIXTUBE_");

// Configure logging.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient<RestService>(configureClient => configureClient.BaseAddress = new Uri($"{GATEWAY_SCHEME}://{GATEWAY_HOST}:{GATEWAY_PORT}"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseStaticFiles();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

// Health check.
app.MapGet("/health", async () => { await Task.Delay(0); return Results.Ok(); });

int WEB_PORT = app.Configuration.GetValue<int>("WEB_PORT");
Console.WriteLine($"Microservice online and listening on port {WEB_PORT}.");
app.Run($"http://0.0.0.0:{WEB_PORT}");