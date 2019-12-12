using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Utils;
using FluentValidation;
using Guards;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Courses.Notification {
    public class StudentNotifyCommand : IRequest<bool> {
        public class Validator : AbstractValidator<StudentNotifyCommand> {
            public Validator() {
                RuleFor(it => it.StudentEmail)
                    .NotNull()
                    .NotEmpty();
                RuleFor(it => it.CourseTitle)
                    .NotNull()
                    .NotEmpty();
            }
        }
        
        public string StudentEmail { get; set; }
        public string CourseTitle { get; set; }
        public string Error { get; set; }
    }

    public class StudentNotifyCommandHandler : IRequestHandler<StudentNotifyCommand, bool> {
        public StudentNotifyCommandHandler(ILogger<StudentNotifyCommandHandler> logger) {
            Guard.NotNull(logger, nameof(logger));

            _logger = logger;
        }

        readonly ILogger<StudentNotifyCommandHandler> _logger;

        public Task<bool> Handle(StudentNotifyCommand command, CancellationToken ct) {
            if (command.Error != null) {
                _logger.LogWarning(command.Error);
            } else {
                _logger.LogInformation($"Student {command.StudentEmail} sucessfully logged in to the {command.CourseTitle} course");
            }
            return Task.FromResult(true);
        }
    }
}