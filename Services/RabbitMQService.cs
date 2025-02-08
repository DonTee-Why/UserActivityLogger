using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Configuration;
using UserActivityLogger.Settings;
using Microsoft.Extensions.Options;

namespace UserActivityLogger.Service
{
    public class RabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _settings;

        public RabbitMQService(IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                AutomaticRecoveryEnabled = true
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(_settings.QueueName, durable: true, exclusive: false, autoDelete: false);
        }

        public void PublishMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: _settings.QueueName, basicProperties: null, body: body);
        }
    }
}