using System.Threading;
using System.Threading.Tasks;
using Courses.Utils;
using Courses.Worker.Commands;
using Guards;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Courses.Worker {
    public class WorkerService : BackgroundService {
        public WorkerService(IMessageReceiver messageReceiver, IMediator mediator) {
            Guard.NotNull(messageReceiver, nameof(messageReceiver));
            Guard.NotNull(mediator, nameof(mediator));

            _mediator = mediator;
            _messageReceiver = messageReceiver;
        }

        readonly IMediator _mediator;
        readonly IMessageReceiver _messageReceiver;

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            stoppingToken.ThrowIfCancellationRequested();

            _messageReceiver.Subscribe(messageBody => {
                System.Console.WriteLine("HANDLED");
                var command = Helpers.DeserializeObject<StudentLogInCommand>(messageBody);
                Task.Run(() => _mediator.Send(command)).Wait();
            });

            return Task.CompletedTask;
        }

        public override void Dispose() {
            _messageReceiver.Dispose();
            
            base.Dispose();
        }
    }
}