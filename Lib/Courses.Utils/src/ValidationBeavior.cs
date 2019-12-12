using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;

namespace Courses.Utils {
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> {
        public ValidationBehavior(AbstractValidator<TRequest> validator) {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        readonly AbstractValidator<TRequest> _validator;
        
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next) {
            await _validator.ValidateAndThrowAsync(request);
            
            return await next();
        }
    }
}