using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Courses.Api.Commands {
    public class StudentEnrollCommandV1 : StudentEnrollCommandBase {
        public class Validator : StudentEnrollCommandBase.Validator<StudentEnrollCommandV1> {
        }
    }

    public class StudentEnrollCommandHandlerV1 : IRequestHandler<StudentEnrollCommandV1, bool> {
        public StudentEnrollCommandHandlerV1() {
        }

        public Task<bool> Handle(StudentEnrollCommandV1 command, CancellationToken ct) {
            return Task.FromResult(true);
        }
    }
}