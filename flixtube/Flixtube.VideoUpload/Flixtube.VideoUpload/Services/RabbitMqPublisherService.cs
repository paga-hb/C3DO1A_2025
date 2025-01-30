using System.Text;
using System.Text.Json;
using Flixtube.VideoUpload.Messages;
using RabbitMQ.Client;

namespace Flixtube.VideoUpload.Services;

public class RabbitMqPublisherService : IRabbitMqPublisherService, IDisposable
{
    private readonly ILogger<RabbitMqPublisherService> _logger;
    private readonly IConfiguration _config;
    private readonly string _exchangeName = "uploaded"; // "uploaded_exchange";
    private readonly string _routingKey = string.Empty; // "video.uploaded";
    private readonly ConnectionFactory _connectionFactory;
    private IConnection _connection = null!;
    private IModel _channel = null!;
    private readonly string RABBIT_MQ_HOST;

    public RabbitMqPublisherService(ILogger<RabbitMqPublisherService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;

        RABBIT_MQ_HOST = config.GetValue<string>("RABBIT_MQ_HOST")!;

        _logger.LogInformation("RabbitMqPublisherService() called.");

        // Configure RabbitMQ connection
        _logger.LogInformation($"Connecting to RabbitMQ server at {RABBIT_MQ_HOST}.");
        _connectionFactory = new ConnectionFactory { Uri = new Uri(RABBIT_MQ_HOST) };

        // Establish RabbitMQ connection
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _logger.LogInformation("Connected to RabbitMQ.");

        // Declare exchange
        // _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: true);
        _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Fanout);
    }

    public void BroadcastVideoUploadedMessage(string videoId, string filename)
    {
        _logger.LogInformation($"BroadcastVideoUploadedMessage({videoId},{filename}) called.");
        
        var msg = new VideoUploadedMessage {Id = videoId, Name = filename};
        string jsonMsg = JsonSerializer.Serialize<VideoUploadedMessage>(msg);
        
        _logger.LogInformation($"Publishing message on '{_exchangeName}' exchange.");

        // Publish message to the "uploaded" exchange.
        _channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: _routingKey,
            basicProperties: null,
            body: Encoding.UTF8.GetBytes(jsonMsg));
    }

    public void Dispose()
    {
        // Cleanup resources
        _channel?.Close();
        _connection?.Close();
    }
}