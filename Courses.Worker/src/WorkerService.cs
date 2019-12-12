using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Domain;
using Courses.Utils;
using Courses.Worker.Commands;
using Guards;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Hosting;
using System.Linq;

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

            _messageReceiver.Subscribe<StudentLogInCommand>(async command => {
                try {
                    await _mediator.Send(command);
                } catch (DomainException e) {
                    System.Console.WriteLine(e.Message);
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