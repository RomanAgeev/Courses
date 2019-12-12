using System.Threading;
using System.Threading.Tasks;
using Courses.Utils;
using Guards;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Hosting;
using System.Linq;

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

            _messageReceiver.Subscribe<StudentNotifyCommand>(async command => {
                try {
                    await _mediator.Send(command);
                } catch (ValidationException e) {
                    System.Console.WriteLine(e.Errors.Select(it => it.ErrorMessage).ToArray());
                }
            });

            return Task.CompletedTask;
        }

        public override void Dispose() {
            _messageReceiver.Dispose();
            
            base.Dispose();
        }
    }
}