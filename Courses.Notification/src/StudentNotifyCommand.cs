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
                RuleFor(it => it.StudentName)
                    .NotNull()
                    .NotEmpty();
                RuleFor(it => it.CourseTitle)
                    .NotNull()
                    .NotEmpty();
                RuleFor(it => it.LogInResult)
                    .NotNull()
                    .NotEmpty();
            }
        }
        
        public string StudentName { get; set; }
        public string CourseTitle { get; set; }
        public string LogInResult { get; set; }
    }

    public class StudentNotifyCommandHandler : IRequestHandler<StudentNotifyCommand, bool> {
        public Task<bool> Handle(StudentNotifyCommand command, CancellationToken ct) {
            switch (command.LogInResult) {
                case LogInResults.Succeed:
                    Console.WriteLine($"Student {command.StudentName} sucessfully logged in to the {command.CourseTitle} course");
                    break;

                case LogInResults.NoCourseCapacity:
                    Console.WriteLine($"Student {command.StudentName} failed to log in to the {command.CourseTitle} course - no enough capacity");
                    break;

                case LogInResults.AlreadyInCourse:
                    Console.WriteLine($"Student {command.StudentName} is already logged in to the {command.CourseTitle} course");
                    break;
            }

            // TODO: Handle unknown result

            return Task.FromResult(true);
        }
    }
}