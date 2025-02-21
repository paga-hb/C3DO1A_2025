using Flixtube.Web.Components;
using Flixtube.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Make sure the necessary Flixtube settings are available in appsettings.json.

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var flixtubeSection = builder.Configuration.GetSection("Flixtube");

if (string.IsNullOrEmpty(flixtubeSection["FLIXTUBE_WEB_PORT"])) {
    throw new Exception("Please specify the port number for Flixtube.Web with the subkey FLIXTUBE_WEB_PORT.");
}

if (string.IsNullOrEmpty(flixtubeSection["FLIXTUBE_GATEWAY_SCHEME"])) {
    throw new Exception("Please specify the scheme for Flixtube.Gateway with the subkey FLIXTUBE_GATEWAY_SCHEME.");
}

if (string.IsNullOrEmpty(flixtubeSection["FLIXTUBE_GATEWAY_HOST"])) {
    throw new Exception("Please specify the host for Flixtube.Gateway with the subkey FLIXTUBE_GATEWAY_HOST.");
}

if (string.IsNullOrEmpty(flixtubeSection["FLIXTUBE_GATEWAY_PORT"])) {
    throw new Exception("Please specify the port number for Flixtube.Gateway with the subkey FLIXTUBE_GATEWAY_PORT.");
}

// Get necessary settings
// Note that we only need to get settings here if there are need before builder.Build()
// int WEB_PORT = int.Parse(flixtubeSection["FLIXTUBE_WEB_PORT"] ?? "80");
string GATEWAY_SCHEME = flixtubeSection["FLIXTUBE_GATEWAY_SCHEME"] ?? string.Empty;
string GATEWAY_HOST = flixtubeSection["FLIXTUBE_GATEWAY_HOST"] ?? string.Empty;
int GATEWAY_PORT = int.Parse(flixtubeSection["FLIXTUBE_GATEWAY_PORT"] ?? "80");

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

int WEB_PORT = app.Configuration.GetValue<int>("Flixtube:FLIXTUBE_WEB_PORT");
Console.WriteLine($"Web online and listening on port {WEB_PORT}.");
app.Run($"http://0.0.0.0:{WEB_PORT}");