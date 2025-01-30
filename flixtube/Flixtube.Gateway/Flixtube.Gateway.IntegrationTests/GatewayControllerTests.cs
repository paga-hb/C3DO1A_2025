using System.Text.Json;
using Microsoft.Playwright;
using FluentAssertions;
using Xunit.Abstractions;
using Flixtube.Gateway.Models;

namespace Flixtube.Gateway.IntegrationTests;

public class GatewayControllerTests : IClassFixture<PlaywrightFixture>
{
    private readonly IAPIRequestContext _apiContext;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ITestOutputHelper _output;

    public GatewayControllerTests(PlaywrightFixture fixture, ITestOutputHelper output)
    {
        _apiContext = fixture.ApiContext;
        _output = output;
        _serializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
    }

    [Fact]
    public async Task HttpGet_Should_Return_All_Metadata()
    {
        // Arrange

        // Add some metadata to the database
        var expected1 = new Video { Id = Guid.NewGuid().ToString(), Name = "TestVideo1" };
        var expected2 = new Video { Id = Guid.NewGuid().ToString(), Name = "TestVideo2" };
        await _apiContext.PostAsync("api/metadata", new APIRequestContextOptions { DataObject = expected1 });
        await _apiContext.PostAsync("api/metadata", new APIRequestContextOptions { DataObject = expected2 });

        // Act
        var response = await _apiContext.GetAsync("api/metadata");
        
        // Assert
        response.Status.Should().Be(200);

        var videos = JsonSerializer.Deserialize<List<Video>>(await response.TextAsync(), _serializerOptions);
        videos.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(videos);
        videos.Should().BeOfType<List<Video>>();

        var actual1 = videos.FirstOrDefault(v => v.Id == expected1.Id);
        actual1?.Id.Should().Be(expected1.Id);
        actual1?.Name.Should().Be(expected1.Name);

        var actual2 = videos.FirstOrDefault(v => v.Id == expected2.Id);
        actual2?.Id.Should().Be(expected2.Id);
        actual2?.Name.Should().Be(expected2.Name);

        _output.WriteLine($"items: Count={videos.Count}");
    }

    [Fact]
    public async Task HttpGet_ForSpecificVideo_Should_Return_Video_Metadata()
    {
        // Arrange

        // Add video metadata to the database
        var expected = new Video { Id = Guid.NewGuid().ToString(), Name = "TestVideo" };
        await _apiContext.PostAsync("api/metadata", new APIRequestContextOptions { DataObject = expected });

        // Act
        var response = await _apiContext.GetAsync($"api/metadata/{expected.Id}");
        
        // Assert
        response.Status.Should().Be(200);

        var actual = JsonSerializer.Deserialize<Video>(await response.TextAsync(), _serializerOptions);
        actual.Should().NotBeNull();
        ArgumentNullException.ThrowIfNull(actual);

        actual.Should().BeOfType<Video>();
        actual.Id.Should().Be(expected.Id);
        actual.Name.Should().Be(expected.Name);
        _output.WriteLine($"Video: Id={actual.Id}, Name={actual.Name}");
    }

    [Fact]
    public async Task HttpPost_Should_Return_Newly_Added_Metadata()
    {
        // Arrange
        var expected = new Video { Id = Guid.NewGuid().ToString(), Name = "TestVideo" };

        // Act
        var response = await _apiContext.PostAsync("api/metadata", new APIRequestContextOptions { DataObject = expected });
        
        // Assert
        response.Status.Should().Be(200);
        _output.WriteLine($"Video: Id={expected.Id}, Name={expected.Name}");
    }

    [Fact]
    public async Task HttpDelete_Should_Return_Newly_Deleted_Metadata()
    {
        // Arrange

        // Add video metadata to the database
        var expected = new Video { Id = Guid.NewGuid().ToString(), Name = "TestVideo" };
        await _apiContext.PostAsync("api/metadata", new APIRequestContextOptions { DataObject = expected });

        // Act
        var response = await _apiContext.DeleteAsync($"api/metadata/{expected.Id}");
        
        // Assert
        response.Status.Should().Be(200);
        _output.WriteLine($"Video: Id={expected.Id}, Name={expected.Name}");
    }
}