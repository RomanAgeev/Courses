using Courses.Api.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers {
    [Route("api/v1/enrollment")]
    [ApiController]
    public class EnrollmentControllerV1 : EnrollmentControllerBase<StudentEnrollCommandV1> {
        public EnrollmentControllerV1(IMediator mediator)
            : base(mediator) {
        }
    }
}