using Microsoft.Playwright;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace Flixtube.Web.EndToEndTests;

public class FlixtubeTests : IClassFixture<PlaywrightFixture>
{
    private readonly IConfiguration _configuration;
    private readonly string _baseUrl;
    private readonly IPlaywright _playwright;
    private readonly IBrowser _chromiumBrowserBrowser;
    private readonly IBrowser _firefoxBrowserBrowser;
    private readonly IBrowser _webKitBrowserBrowser;
    private readonly ITestOutputHelper _output;

    public FlixtubeTests(PlaywrightFixture fixture, ITestOutputHelper output)
    {
        _output = output;
        _configuration = fixture.Configuration;
        _baseUrl = fixture.BaseUrl;
        _playwright = fixture.Playwright;
        _chromiumBrowserBrowser = fixture.ChromiumBrowser;
        _firefoxBrowserBrowser = fixture.FirefoxBrowser;
        _webKitBrowserBrowser = fixture.WebkitBrowser;
    }

    [Fact]
    public async Task HomePage_NavigateTo_Title_ShouldContain_VideoList()
    {
        // Add testing logic here
        
        // Open a new browser
        var context = await _chromiumBrowserBrowser.NewContextAsync();  // Use this if you want to test in a Chromium-based browser
        //var context = await _firefoxBrowserBrowser.NewContextAsync(); // Use this if you want to test in a Firefox-based browser
        //var context = await _webKitBrowserBrowser.NewContextAsync();  // Use this if you want to test in a WebKit-based browser

         // Create a Page object (we can use this to navigate to different URLs, and interact with HTML elements on web pages)
        var page = await context.NewPageAsync();

        // Navigate to the Home Page
        await page.GotoAsync(_baseUrl, new PageGotoOptions { WaitUntil = WaitUntilState.Load });
        
        // Assert the title contains text "Video List"
        var title = await page.TitleAsync();
        Console.WriteLine($"Page title: {title}");
        title.Should().Contain("Video List");
        _output.WriteLine($"Home Page Title is Video List");

        // Save a screenshot of the Home page
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "/screenshots/home_page_screenshot.png" });
        // var home_page_content = await page.ContentAsync();
        // _output.WriteLine($"Home Page HTML: {home_page_content}");

        // Click the "Show Viewing History" button, which will navigate to the Viewing History page
        await page.GetByRole(AriaRole.Button, new() { Name = "Show Viewing History" }).ClickAsync();
        await Task.Delay(500);

        // Assert the title contains text "Viewing History"
        title = await page.TitleAsync();
        title.Should().Contain("Viewing History");

        // Save a screenshot of the Viewing History page
        await page.ScreenshotAsync(new PageScreenshotOptions { Path = "/screenshots/viewing_history_page_screenshot.png" });

        // Click the "Home" button, which will navigate back to the Home page
        await page.GetByRole(AriaRole.Button, new() { Name = "Home" }).ClickAsync();
        await Task.Delay(500);

        // Assert the title contains text "Video List"
        title = await page.TitleAsync();
        title.Should().Contain("Video List");

        // Etc ...
        // see: https://playwright.dev/dotnet/docs/intro
        
        // Clean up (closes the web browser) <--- Added this
        await context.CloseAsync();
    }
}