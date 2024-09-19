using Application.Interfaces;
using Domain.Common.Implementations;
using Domain.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

public class RabbitMqEventPublisher : IEventPublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMqEventPublisher(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMqSettings:HostName"],
            Port = int.Parse(configuration["RabbitMqSettings:Port"]),
            UserName = configuration["RabbitMqSettings:UserName"],
            Password = configuration["RabbitMqSettings:Password"]
        };

        _queueName = configuration["RabbitMqSettings:QueueName"];
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        // _channel.QueueDeclare(queue: _queueName,
        //                       durable: false,
        //                       exclusive: false,
        //                       autoDelete: false,
        //                       arguments: null);

        try
        {
            _channel.QueueDeclarePassive(_queueName);
            Console.WriteLine($"Fila {_queueName} já exist.");
        }
        catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
        {
            Console.WriteLine($"Fila {_queueName} não existe, criando fila...");
            if (_channel.IsClosed)
            {
                _channel = _connection.CreateModel();  // Reabertura do canal se estiver fechado
            }
            _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            Console.WriteLine($"Fila {_queueName} criada com sucesso.");
        }
    }

    public async Task<IResult<bool>> PublishAsync<T>(T @event) where T : class
    {
        // var message = JsonSerializer.Serialize(@event);
        // var body = Encoding.UTF8.GetBytes(message);

        // _channel.BasicPublish(exchange: "",
        //                       routingKey: _queueName,
        //                       basicProperties: null,
        //                       body: body);
        // return Task.CompletedTask;
        try
        {
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: _queueName,
                                  basicProperties: null,
                                  body: body);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail(new List<string> { ex.Message }, false);
        }
    }
}