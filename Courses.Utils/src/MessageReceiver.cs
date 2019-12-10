using System;
using Guards;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Courses.Utils {
    public interface IMessageReceiver : IDisposable {
        void Subscribe(Action<byte[]> handler);
    }

    public class MessageReceiver : IMessageReceiver {
        public MessageReceiver(string queueName) {
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

        public void Subscribe(Action<byte[]> handler) {
            Guard.NotNull(handler, nameof(handler));

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (ch, ea) => {
                handler(ea.Body);

                // TODO: Ack
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: true,
                consumer: consumer
            );
        }

        public void Dispose() {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}