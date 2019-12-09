using System;
using System.Net;
using System.Threading.Tasks;
using Courses.Api.Commands;
using Guards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers {
    [Route("api/v2/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase {
        public StudentController(IMediator mediator) {
            Guard.NotNull(mediator, nameof(mediator));
            
            _mediator = mediator;
        }

        readonly IMediator _mediator;

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Post(StudentLogInCommand command) {
            await _mediator.Send(command);

            return NoContent();
        }
    }
}
