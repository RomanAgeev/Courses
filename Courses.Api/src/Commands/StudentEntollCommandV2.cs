using System.Threading;
using System.Threading.Tasks;
using Courses.Utils;
using Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Courses.Api.Commands {
    public class StudentEnrollCommandV2 : StudentEnrollCommandBase {
        public class Validator : StudentEnrollCommandBase.Validator<StudentEnrollCommandV2> {
        }
    }

     public class StudentEnrollCommandHandlerV2 : IRequestHandler<StudentEnrollCommandV2, bool> {
        public StudentEnrollCommandHandlerV2(IMessageSender messageSender, ILogger<StudentEnrollCommandHandlerV2> logger) {
            Guard.NotNull(messageSender, nameof(messageSender));
            Guard.NotNull(logger, nameof(logger));
            
            _messageSender = messageSender;
            _logger = logger;
        }

        readonly IMessageSender _messageSender;
        readonly ILogger<StudentEnrollCommandHandlerV2> _logger;

        public Task<bool> Handle(StudentEnrollCommandV2 command, CancellationToken ct) {
            _logger.LogInformation($"Sending message to {_messageSender.QueueName}");

            _messageSender.SendMessage(new {
                command.StudentEmail,
                command.CourseTitle
            });

            return Task.FromResult(true);
        }
    }
}