using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Guards;
using Courses.Utils;

namespace Courses.Api.Services {
    public interface IMessageService {
        Task SendMessage(object payload);
    }

    public class MessageService : IMessageService, IDisposable {
        public MessageService() {
            var factory = new ConnectionFactory {
                HostName = "localhost"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        readonly IConnection _connection;
        readonly IModel _channel;

        public Task SendMessage(object payload) {
            Guard.NotNull(payload, nameof(payload));

            byte[] body = Helpers.SerializeObject(payload);

            _channel.BasicPublish(
                exchange: "",
                routingKey: "hello",
                basicProperties: null,
                body: body
            );

            return Task.CompletedTask;
        }

        public void Dispose() {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}