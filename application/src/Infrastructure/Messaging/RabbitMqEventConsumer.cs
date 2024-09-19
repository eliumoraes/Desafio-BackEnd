using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Messagin;

public class RabbitMqEventConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;

    public RabbitMqEventConsumer(IConfiguration configuration)
    {
        // Connection com RabbitMQ
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

        // Verificar se a fila existe, se não existir, criá-la
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

    // Método para começar a consumir mensagens
    public void StartConsuming()
    {
        var consumer = new EventingBasicConsumer(_channel);

        // Callback executado quando uma nova mensagem chega na fila
        consumer.Received += (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }

            // Executar lógica adicional daqui... banco etc...
        };

        // Inicia o consumo da fila, com auto-acknowledge ativado
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
    }
}