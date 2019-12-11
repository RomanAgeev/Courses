using System.Net;
using System.Threading.Tasks;
using Courses.Api.Commands;
using Guards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers {
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StudentController : CourseControllerBase {
        public StudentController(IMediator mediator)
            : base(mediator) {
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Post(StudentInsertCommand command) {
            await Mediator.Send(command);

            return NoContent();
        }
    }
}
