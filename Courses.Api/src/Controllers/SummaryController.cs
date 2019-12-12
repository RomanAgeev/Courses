using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Courses.Api.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers {
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SummaryController : CourseControllerBase {
        public SummaryController(IMediator mediator)
            :base(mediator) {
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CourseSummaryListModel>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCourses() {
            var models = await Mediator.Send(new CourseSummaryListQuery());

            return Ok(models);
        }
    }
}