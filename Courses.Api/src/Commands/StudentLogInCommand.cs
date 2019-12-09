using System;
using System.Threading;
using System.Threading.Tasks;
using Courses.Api.Services;
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
        public StudentLogInCommandHandler(IMessageService messageService) {
            Guard.NotNull(messageService, nameof(messageService));
            
            _messageService = messageService;
        }

        readonly IMessageService _messageService;

        public async Task<bool> Handle(StudentLogInCommand command, CancellationToken ct) {
            await _messageService.SendMessage(new {
                command.StudentName,
                command.CourseTitle
            });

            return true;
        }
    }
}