namespace Flixtube.VideoUpload.Services;

public interface IRabbitMqPublisherService
{
    void BroadcastVideoUploadedMessage(string videoId, string filename);
}