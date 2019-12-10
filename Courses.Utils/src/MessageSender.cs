using System;
using Guards;
using RabbitMQ.Client;

namespace Courses.Utils {
    public interface IMessageSender : IDisposable {
        void SendMessage(object payload);
    }

    public class MessageSender : IMessageSender {
        public MessageSender(string queueName) {
            Guard.NotNullOrEmpty(queueName, nameof(queueName));

            _queueName = queueName;

            var factory = new ConnectionFactory { HostName = "localhost" };

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

        public void SendMessage(object payload) {
            Console.WriteLine($" [x] Sending to {_queueName}");

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
