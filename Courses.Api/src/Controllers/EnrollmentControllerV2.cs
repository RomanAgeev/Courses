using Courses.Api.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers {
    [Route("api/v2/enrollment")]
    [ApiController]
    public class EnrollmentControllerV2 : EnrollmentControllerBase<StudentEnrollCommandV2> {
        public EnrollmentControllerV2(IMediator mediator)
            : base(mediator) {
        }
    }
}