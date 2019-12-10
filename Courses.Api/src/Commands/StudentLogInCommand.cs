using System.Threading;
using System.Threading.Tasks;
using Courses.Utils;
using FluentValidation;
using Guards;
using MediatR;

namespace Courses.Api.Commands {
    public class StudentLogInCommand : IRequest<bool> {
        public class Validator : AbstractValidator<StudentLogInCommand> {
            public Validator() {
                RuleFor(it => it.StudentName)
                    .NotNull()
                    .NotEmpty();
                RuleFor(it => it.CourseTitle)
                    .NotNull()
                    .NotEmpty();
            }
        }

        public string StudentName { get; set; }
        public string CourseTitle { get; set; } 
    }

    public class StudentLogInCommandHandler : IRequestHandler<StudentLogInCommand, bool> {
        public StudentLogInCommandHandler(IMessageSender messageSender) {
            Guard.NotNull(messageSender, nameof(messageSender));
            
            _messageSender = messageSender;
        }

        readonly IMessageSender _messageSender;

        public Task<bool> Handle(StudentLogInCommand command, CancellationToken ct) {
            _messageSender.SendMessage(new {
                command.StudentName,
                command.CourseTitle
            });

            return Task.FromResult(true);
        }
    }
}