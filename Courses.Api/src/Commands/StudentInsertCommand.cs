using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Domain;
using FluentValidation;
using Guards;
using MediatR;

namespace Courses.Api.Commands {
    public class StudentInsertCommand : IRequest<bool> {
        public class Validator : AbstractValidator<StudentInsertCommand> {
            public Validator() {
                RuleFor(it => it.Email)
                    .EmailAddress();
                RuleFor(it => it.Name)
                    .NotNull()
                    .NotEmpty();
                RuleFor(it => it.Age)
                    .GreaterThan(0);
            }
        }

        public string Email { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class StudentInsertCommandHandler : IRequestHandler<StudentInsertCommand, bool> {
        public StudentInsertCommandHandler(IStudentRepository repository) {
            Guard.NotNull(repository, nameof(repository));

            _repository = repository;
        }

        readonly IStudentRepository _repository;

        public async Task<bool> Handle(StudentInsertCommand command, CancellationToken ct) {
            Student student = await _repository.GetStudentAsync(command.Email);
            if (student != null) {
                throw new Exception("TODO");
            }

            student = new Student(command.Name, command.Age, command.Email);

            await _repository.InsertStudentAsync(student);

            return true;
        }
    }
}