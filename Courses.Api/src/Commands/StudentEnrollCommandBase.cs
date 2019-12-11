using FluentValidation;
using MediatR;

namespace Courses.Api.Commands {
    public abstract class StudentEnrollCommandBase : IRequest<bool> {
        public abstract class Validator<T> : AbstractValidator<T> where T : StudentEnrollCommandBase {
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
}