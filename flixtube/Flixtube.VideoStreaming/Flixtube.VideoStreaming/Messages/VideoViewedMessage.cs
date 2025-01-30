namespace Flixtube.VideoStreaming.Messages;

public class VideoViewedMessage
{
    public string VideoId { get; set; } = null!;
    public DateTime ViewedAt { get; set; }
}