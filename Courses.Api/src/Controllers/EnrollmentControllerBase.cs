using System.Net;
using System.Threading.Tasks;
using Courses.Api.Commands;
using Guards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers {
    public abstract class EnrollmentControllerBase<T> : CourseControllerBase where T : StudentEnrollCommandBase {
        protected EnrollmentControllerBase(IMediator mediator)
            : base(mediator) {
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Post(T command) {
            await Mediator.Send(command);

            return NoContent();
        }
    }
}