using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;
using Flixtube.Metadata.Entities;
using Flixtube.Metadata.Repositories;
using Flixtube.Metadata.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Flixtube.Metadata.UnitTests;

public class MetadataControllerTests
{
    private readonly List<Video> _videos;
    private readonly Video _video1;
    private readonly Mock<IVideoRepository> _mockVideoRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<MetadataController>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly MetadataController _metadataController;
    private readonly ITestOutputHelper _output;

    public MetadataControllerTests(ITestOutputHelper output)
    {
        _output = output;

        // Test Fixture

        // Sample data
        _videos = new()
        {
            new Video() { Id = "5d9e690ad76fe06a3d7ae416", Name = "SampleVideo_1280x720_1mb.mp4" },
            new Video() { Id = "1asd098f9a00sf98f098asd9", Name = "SampleVideo2_1280x720_1mb.mp4" }
        };
        _video1 = _videos[0];
        string Id = _video1.Id;

        // Mock VideoRepository
        _mockVideoRepository = new Mock<IVideoRepository>();
        _mockVideoRepository.Setup(r => r.FindAsync()).ReturnsAsync(() => _videos);
        _mockVideoRepository.Setup(r => r.FirstOrDefaultAsync(v => v.Id == Id)).ReturnsAsync(() => _video1);
        _mockVideoRepository.Setup(r => r.Add(_video1));
        _mockVideoRepository.Setup(r => r.Update(It.IsAny<Video>()));
        _mockVideoRepository.Setup(r => r.Remove(_video1));

        // Mock UnitOfWork
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUnitOfWork.Setup(u => u.Videos).Returns(_mockVideoRepository.Object);
        _mockUnitOfWork.Setup(u => u.CompleteAsync()).ReturnsAsync(() => It.IsAny<int>());

        // Mock Logger
        _mockLogger = new Mock<ILogger<MetadataController>>();
        _mockLogger.Setup(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>())
        );

        // Mock Configuration
        _mockConfig = new Mock<IConfiguration>();
        // _mockConfig.Setup(c => c["MyEnvironmentVariable"]).Returns("MockedValue");

        // Subject Under Test (SUT)
        _metadataController = new MetadataController(_mockLogger.Object, _mockConfig.Object, _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task GetVideos_ForExistingVideos_ShouldReturnVideos()
    {
        // Arrange
        List<Video> expected = _videos;

        // Act
        IActionResult result = await _metadataController.GetVideos();

        // Assert

        // Verify correct status code 
        OkObjectResult ok = (OkObjectResult)result;
        ok.StatusCode.Should().Be(200);

        // Verify correct response
        List<Video> actual = (List<Video>)ok.Value!;
        actual.Count.Should().Be(expected.Count);
        for (int idx=0; idx < actual.Count; idx++)
        {
            actual[idx].Id.Should().Be(expected[idx].Id);
            actual[idx].Name.Should().Be(expected[idx].Name);
        }  
        _output.WriteLine($"GetVideos(): Count={actual.Count}");

        // Verify UnitOfWork.Videos.FindAsync() was called exactly once
        _mockUnitOfWork.Verify(c => c.Videos.FindAsync(), Times.Once());
    }

    [Fact]
    public async Task GetVideo_ForExistingId_ShouldReturnVideo()
    {
        // Arrange
        string id = _video1.Id;
        Video expected = _video1;

        // Act
        IActionResult result = await _metadataController.GetVideo(id);

        // Assert

        // Verify correct status code 
        OkObjectResult ok = (OkObjectResult)result;
        ok.StatusCode.Should().Be(200);

        // Verify correct response
        Video actual = (Video)ok.Value!;
        actual.Id.Should().Be(expected.Id);
        actual.Name.Should().Be(expected.Name);
        _output.WriteLine($"GetVideo({id}): Id={actual.Id}, Name={actual.Name}");

        // Verify UnitOfWork.Videos.FirstOrDefaultAsync() was called exactly once
        _mockUnitOfWork.Verify(c => c.Videos.FirstOrDefaultAsync(v => v.Id == id), Times.Once());
    }

    [Fact]
    public async Task AddVideo_ForNonExistingId_ShouldReturnNewlyCreatedVideo()
    {
        // Arrange
        string id = _video1.Id;
        Video expected = _video1;

        // Act
        IActionResult result = await _metadataController.AddVideo(expected);

        // Assert

        // Verify correct status code 
        OkObjectResult ok = (OkObjectResult)result;
        ok.StatusCode.Should().Be(200);

        // Verify correct response
        Video actual = (Video)ok.Value!;
        actual.Id.Should().Be(expected.Id);
        actual.Name.Should().Be(expected.Name);
        _output.WriteLine($"AddVideo({id}): Id={actual.Id}, Name={actual.Name}");

        // Verify UnitOfWork.Videos.Add() and UnitOfWork.CompleteAsync() were called exactly once
        _mockUnitOfWork.Verify(c => c.Videos.Add(It.IsAny<Video>()), Times.Once());
        _mockUnitOfWork.Verify(c => c.CompleteAsync(), Times.Once());
    }

    [Fact]
    public async Task DeleteVideo_ForExistingId_ShouldReturnDeletedVideo()
    {
        // Arrange
        string id = _video1.Id;
        Video expected = _video1;

        // Act
        IActionResult result = await _metadataController.DeleteVideo(id);

        // Assert

        // Verify correct status code 
        OkObjectResult ok = (OkObjectResult)result;
        ok.StatusCode.Should().Be(200);

        // Verify correct response
        Video actual = (Video)ok.Value!;
        actual.Id.Should().Be(expected.Id);
        actual.Name.Should().Be(expected.Name);
        _output.WriteLine($"DeleteVideo({id}): Id={actual.Id}, Name={actual.Name}");

        // Verify UnitOfWork.Videos.FirstOrDefaultAsync() was called exactly once
        _mockUnitOfWork.Verify(c => c.Videos.FirstOrDefaultAsync(v => v.Id == id), Times.Once());
        
        // Verify UnitOfWork.Videos.Remove() and UnitOfWork.CompleteAsync() were called exactly once
        _mockUnitOfWork.Verify(c => c.Videos.Remove(It.IsAny<Video>()), Times.Once());
        _mockUnitOfWork.Verify(c => c.CompleteAsync(), Times.Once());
    }
}