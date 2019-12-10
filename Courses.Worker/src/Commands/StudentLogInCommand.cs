using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Domain;
using Courses.Utils;
using FluentValidation;
using Guards;
using MediatR;

namespace Courses.Worker.Commands {
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
        public StudentLogInCommandHandler(ICourseRepository repository, IMessageSender messageSender) {
            Guard.NotNull(repository, nameof(repository));
            Guard.NotNull(messageSender, nameof(messageSender));

            _repository = repository;
            _messageSender = messageSender;
        }

        readonly ICourseRepository _repository;
        readonly IMessageSender _messageSender;

        public async Task<bool> Handle(StudentLogInCommand command, CancellationToken ct) {
            bool suceed;

            do {
                Course course = await _repository.GetCourseAsync(command.CourseTitle);
                if (course == null) {
                    throw new Exception("TODO");
                }

                int version = course.Version;

                try {
                    course.AddStudent(command.StudentName);
                } catch {
                    return false;
                }

                suceed = await _repository.SetCourseAsync(version, course);
            } while(!suceed);

            _messageSender.SendMessage(new {
                command.CourseTitle,
                command.StudentName
            });
            
            return true;
        }
    }
}