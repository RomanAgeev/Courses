using FluentValidation;
using MediatR;

namespace Courses.Api.Commands {
    public abstract class StudentEnrollCommandBase : IRequest<bool> {
        public abstract class Validator<T> : AbstractValidator<T> where T : StudentEnrollCommandBase {
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
    }
}