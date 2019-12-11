using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Domain;
using FluentValidation;
using Guards;
using MediatR;

namespace Courses.Api.Commands {
    public class CourseInsertCommand : IRequest<bool> {
         public class Validator : AbstractValidator<CourseInsertCommand> {
            public Validator() {
                RuleFor(it => it.Title)
                    .NotNull()
                    .NotEmpty();
                RuleFor(it => it.Teacher)
                    .NotNull()
                    .NotEmpty();
                RuleFor(it => it.Capacity)
                    .GreaterThan(0);
            }
        }

        public string Title { get; set; }
        public string Teacher { get; set; }
        public int Capacity { get; set; }
    }

     public class CourseInsertCommandHandler : IRequestHandler<CourseInsertCommand, bool> {
        public CourseInsertCommandHandler(ICourseRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly ICourseRepository _repository;

        public async Task<bool> Handle(CourseInsertCommand command, CancellationToken ct) {
            Course course = await _repository.GetCourseAsync(command.Title);
            if (course != null) {
                throw new DomainException($"Course '{command.Title}' already exists");
            }

            course = new Course(command.Title, command.Teacher, command.Capacity);

            await _repository.InsertCourseAsync(course);

            return true;
        }
     }
}