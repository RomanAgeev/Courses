using System.Threading;
using System.Threading.Tasks;
using Courses.Utils;
using Guards;
using MediatR;

namespace Courses.Api.Commands {
    public class StudentEnrollCommandV2 : StudentEnrollCommandBase {
        public class Validator : StudentEnrollCommandBase.Validator<StudentEnrollCommandV2> {
        }
    }

     public class StudentEnrollCommandHandlerV2 : IRequestHandler<StudentEnrollCommandV2, bool> {
        public StudentEnrollCommandHandlerV2(IMessageSender messageSender) {
            Guard.NotNull(messageSender, nameof(messageSender));
            
            _messageSender = messageSender;
        }

        readonly IMessageSender _messageSender;

        public Task<bool> Handle(StudentEnrollCommandV2 command, CancellationToken ct) {
            System.Console.WriteLine("V2");

            _messageSender.SendMessage(new {
                command.StudentName,
                command.CourseTitle
            });

            return Task.FromResult(true);
        }
    }
}