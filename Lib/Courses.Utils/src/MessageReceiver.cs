using System;
using System.Threading.Tasks;
using Guards;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Courses.Utils {
    public interface IMessageReceiver : IDisposable {
        void Subscribe<T>(Func<T, Task> handler, Action<Exception, string> logError,
            Action<Exception, string> logCritical) where T : class;
    }

    public class MessageReceiver : IMessageReceiver {
        public MessageReceiver(string hostName, string queueName) {
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

        public void Subscribe<T>(Func<T, Task> handler, Action<Exception, string> logError,
            Action<Exception, string> logCritical) where T : class {

            Guard.NotNull(handler, nameof(handler));

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (ch, ea) => {
                T data = null;
                try {
                    data = Helpers.DeserializeObject<T>(ea.Body);
                } catch(Exception e) {
                    logError(e, $"Invalid data format '{e.Message}'");
                }

                if (data != null) {
                    try {
                    await handler(data);
                    } catch(Exception e) {
                        logCritical(e, $"Unhandled exception '{e.Message}'");
                        throw;
                    }
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(
                queue: _queueName,
                autoAck: false,
                consumer: consumer
            );
        }

        public void Dispose() {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}