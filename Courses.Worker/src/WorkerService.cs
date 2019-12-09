using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Courses.Domain;
using Courses.Utils;
using Courses.Worker.Commands;
using Guards;
using MediatR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Courses.Worker {
    public class WorkerService : BackgroundService {
        public WorkerService(IMediator mediator, ICourseRepository repository) {
            Guard.NotNull(mediator, nameof(mediator));
            Guard.NotNull(repository, nameof(repository));

            _mediator = mediator;
            _repository = repository;

            var factory = new ConnectionFactory {
                HostName = "localhost",
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        readonly IMediator _mediator;
        readonly ICourseRepository _repository;
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
            consumer.Received += async (ch, ea) => {
                var command = Helpers.DeserializeObject<StudentLogInCommand>(ea.Body);

                await _mediator.Send(command); 

                // TODO: Ack
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