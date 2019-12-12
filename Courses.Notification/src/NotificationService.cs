using System.Threading;
using System.Threading.Tasks;
using Courses.Utils;
using Guards;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Courses.Notification {
    public class NotificationService : BackgroundService {
        public NotificationService(IMessageReceiver messageReceiver, IMediator mediator, ILogger<NotificationService> logger) {
            Guard.NotNull(messageReceiver, nameof(messageReceiver));
            Guard.NotNull(mediator, nameof(mediator));
            Guard.NotNull(logger, nameof(logger));

            _mediator = mediator;
            _messageReceiver = messageReceiver;
            _logger = logger;
        }

        readonly IMediator _mediator;
        readonly IMessageReceiver _messageReceiver;
        readonly ILogger<NotificationService> _logger;

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            stoppingToken.ThrowIfCancellationRequested();

            _messageReceiver.Subscribe<StudentNotifyCommand>(async command => {
                try {
                    await _mediator.Send(command);
                } catch (ValidationException e) {
                    _logger.LogWarning(e, "Validation");
                }
            },
            (e, message) => _logger.LogError(e, message),
            (e, message) => _logger.LogCritical(e, message));

            return Task.CompletedTask;
        }

        public override void Dispose() {
            _messageReceiver.Dispose();
            
            base.Dispose();
        }
    }
}