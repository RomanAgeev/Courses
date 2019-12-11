using System.Net;
using System.Threading.Tasks;
using Courses.Api.Commands;
using Guards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers {
    public abstract class EnrollmentControllerBase<T> : ControllerBase where T : StudentEnrollCommandBase {
        protected EnrollmentControllerBase(IMediator mediator) {
            Guard.NotNull(mediator, nameof(mediator));
            
            _mediator = mediator;
        }

        readonly IMediator _mediator;

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Post(T command) {
            await _mediator.Send(command);

            return NoContent();
        }
    }
}