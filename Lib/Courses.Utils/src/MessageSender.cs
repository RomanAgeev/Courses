using System;
using Guards;
using RabbitMQ.Client;

namespace Courses.Utils {
    public interface IMessageSender : IDisposable {
        string QueueName { get; }

        void SendMessage<T>(T payload);
    }

    public class MessageSender : IMessageSender {
        public MessageSender(string hostName, string queueName) {
            Guard.NotNullOrEmpty(hostName, nameof(hostName));
            Guard.NotNullOrEmpty(queueName, nameof(queueName));

            _queueName = queueName;

            var factory = new ConnectionFactory { HostName = hostName };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        readonly string _queueName;
        readonly IConnection _connection;
        readonly IModel _channel;

        public string QueueName => _queueName;

        public void SendMessage<T>(T payload) {
            Guard.NotNull(payload, nameof(payload));

            byte[] body = Helpers.SerializeObject(payload);

            _channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: null,
                body: body
            );
        }

        public void Dispose() {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
