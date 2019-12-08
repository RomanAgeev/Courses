using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Courses.Worker {
    public class WorkerService : BackgroundService {
        public WorkerService() {
            var factory = new ConnectionFactory {
                HostName = "localhost",
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        readonly IConnection _connection;
        readonly IModel _channel;

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            stoppingToken.ThrowIfCancellationRequested();

            _channel.QueueDeclare(
                queue: "hello",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) => {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };

            _channel.BasicConsume(
                queue: "hello",
                autoAck: true,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        public override void Dispose() {
            _channel.Dispose();
            _connection.Dispose();

            base.Dispose();
        }
    }
}