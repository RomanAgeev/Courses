using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;

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
            if (payload == null) {
                throw new ArgumentNullException(nameof(payload));
            }

            string json = JsonConvert.SerializeObject(payload);
            byte[] body = Encoding.UTF8.GetBytes(json);

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