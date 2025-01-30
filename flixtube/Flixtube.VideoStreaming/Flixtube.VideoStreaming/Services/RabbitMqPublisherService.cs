using System.Text;
using System.Text.Json;
using Flixtube.VideoStreaming.Messages;
using RabbitMQ.Client;

namespace Flixtube.VideoStreaming.Services;

public class RabbitMqPublisherService : IRabbitMqPublisherService, IDisposable
{
    private readonly ILogger<RabbitMqPublisherService> _logger;
    private readonly IConfiguration _config;
    private readonly string _exchangeName = "viewed"; // "viewed_exchange";
    private readonly string _routingKey = string.Empty; // "video.viewed";
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

    public void BroadcastVideoViewedMessage(string videoId)
    {
        _logger.LogInformation($"BroadcastVideoViewedMessage({videoId}) called.");
        
        var msg = new VideoViewedMessage {VideoId = videoId, ViewedAt = DateTime.UtcNow};
        string jsonMsg = JsonSerializer.Serialize<VideoViewedMessage>(msg);
        
        _logger.LogInformation($"Publishing message on '{_exchangeName}' exchange.");

        // Publish message to the "viewed" exchange.
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