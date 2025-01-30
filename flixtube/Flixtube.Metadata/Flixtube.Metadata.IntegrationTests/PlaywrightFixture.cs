using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;

namespace Flixtube.Metadata.IntegrationTests;

public class PlaywrightFixture : IAsyncLifetime
{
    public IConfiguration Configuration { get; private set; } = null!;
    public IPlaywright Playwright { get; private set; } = null!;
    public IAPIRequestContext ApiContext { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        
        ApiContext = await Playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            BaseURL = Configuration.GetSection("TestSettings")["ApiBaseUrl"]
        });
    }

    public async Task DisposeAsync()
    {
        await ApiContext.DisposeAsync();
        Playwright.Dispose();
    }
}