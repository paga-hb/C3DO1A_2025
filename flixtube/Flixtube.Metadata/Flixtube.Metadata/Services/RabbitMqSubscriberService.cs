using System.Text;
using System.Text.Json;
using Flixtube.Metadata.Entities;
using Flixtube.Metadata.Messages;
using Flixtube.Metadata.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Flixtube.Metadata.Services;

public class RabbitMqSubscriberService : BackgroundService
{
    private readonly ILogger<RabbitMqSubscriberService> _logger;
    private readonly IConfiguration _config;
    private readonly string _exchangeName = "uploaded"; // "uploaded_exchange";
    private string _queueName = "uploaded_queue";
    private readonly string _routingKey = string.Empty; // "video.uploaded";
    private readonly ConnectionFactory _connectionFactory;
    private IConnection _connection = null!;
    private IModel _channel = null!;
    // private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly string RABBIT_MQ_HOST;

    public RabbitMqSubscriberService(ILogger<RabbitMqSubscriberService> logger, IConfiguration config, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _config = config;
        _serviceScopeFactory = serviceScopeFactory;

        RABBIT_MQ_HOST = config.GetValue<string>("RABBIT_MQ_HOST")!;

        _logger.LogInformation("RabbitMqSubscriberService() called.");

        // Configure RabbitMQ connection
        _logger.LogInformation($"Connecting to RabbitMQ server at {RABBIT_MQ_HOST}.");
        _connectionFactory = new ConnectionFactory { Uri = new Uri(RABBIT_MQ_HOST) };
    }

    // public RabbitMqSubscriberService(ILogger<RabbitMqSubscriberService> logger, IConfiguration config, IUnitOfWork unitOfWork)
    // {
    //     _logger = logger;
    //     _config = config;
    //     _unitOfWork = unitOfWork;

    //     RABBIT_MQ_HOST = config.GetValue<string>("RABBIT_MQ_HOST")!;

    //     _logger.LogInformation("RabbitMqSubscriberService() called.");

    //     // Configure RabbitMQ connection
    //     _logger.LogInformation($"Connecting to RabbitMQ server at {RABBIT_MQ_HOST}.");
    //     _connectionFactory = new ConnectionFactory { Uri = new Uri(RABBIT_MQ_HOST) };
    // }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StartAsync() called.");

        // Establish RabbitMQ connection
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _logger.LogInformation("Connected to RabbitMQ.");

        // Declare exchange and queue
        // _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: true);
        // _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        // _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: _routingKey);
        _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Fanout);
        _queueName = _channel.QueueDeclare(exclusive: true).QueueName; // creates an anonymous queue
        _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: _routingKey);
        _logger.LogInformation($"Created queue {_queueName} and bound it to the '{_exchangeName}' exchange.");

        return base.StartAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExecuteAsync() called.");
        
        // Create a consumer to handle RabbitMQ message events
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            _logger.LogInformation($"Received an '{_exchangeName}' message");

            // Deserialize and process the message
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            // _logger.LogInformation($"Received message: {message}");

            var parsedMsg = JsonSerializer.Deserialize<VideoUploadedMessage>(message); // Parse the JSON message.
            _logger.LogInformation($"VideoUploadedMessage Id: {parsedMsg!.Id}, Name: {parsedMsg.Name}");

            // Store details (metadata) about the newly uploaded video in the Metadata database
            var video = new Video { Id = parsedMsg!.Id, Name = parsedMsg.Name };
            // _logger.LogInformation($"Video Id: {video.Id}, Name: {video.Name}");
            
            using var scope = _serviceScopeFactory.CreateScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _unitOfWork.Videos.Add(video);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Acknowledging message was handled.");
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        // Start consuming messages
        // _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        // Cleanup resources
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}