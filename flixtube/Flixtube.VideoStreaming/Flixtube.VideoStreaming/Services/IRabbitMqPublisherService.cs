namespace Flixtube.VideoStreaming.Services;

public interface IRabbitMqPublisherService
{
    void BroadcastVideoViewedMessage(string videoId);
}