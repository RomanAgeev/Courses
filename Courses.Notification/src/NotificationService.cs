using System.Threading;
using System.Threading.Tasks;
using Courses.Utils;
using Guards;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Courses.Notification {
    public class NotificationService : BackgroundService {
        public NotificationService(IMessageReceiver messageReceiver, IMediator mediator) {
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
                var command = Helpers.DeserializeObject<StudentNotifyCommand>(messageBody);
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