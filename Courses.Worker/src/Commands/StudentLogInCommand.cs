using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
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
        public StudentLogInCommandHandler() {
        }

        public async Task<bool> Handle(StudentLogInCommand command, CancellationToken ct) {
            System.Console.WriteLine("WORKER: StudentLogInCommandHandler");
            return await Task.FromResult(true);
        }
    }
}