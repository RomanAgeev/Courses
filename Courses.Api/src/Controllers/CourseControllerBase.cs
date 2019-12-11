using Guards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers {
    public abstract class CourseControllerBase : ControllerBase {
        protected CourseControllerBase(IMediator mediator) {
            Guard.NotNull(mediator, nameof(mediator));
            
            _mediator = mediator;
        }

        readonly IMediator _mediator;

        protected IMediator Mediator => _mediator;
    }
}