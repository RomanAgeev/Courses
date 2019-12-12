using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Utils;
using FluentValidation;
using MediatR;

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
        public Task<bool> Handle(StudentNotifyCommand command, CancellationToken ct) {
            if (command.Error != null) {
                Console.WriteLine(command.Error);
            } else {
                Console.WriteLine($"Student {command.StudentEmail} sucessfully logged in to the {command.CourseTitle} course");
            }
            return Task.FromResult(true);
        }
    }
}