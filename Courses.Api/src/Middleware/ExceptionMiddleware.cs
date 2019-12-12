using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Courses.Domain;
using FluentValidation;
using Guards;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Courses.Api.Middleware {
    public class ExceptionMiddleware {
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger) {
            Guard.NotNull(next, nameof(next));
            Guard.NotNull(logger, nameof(logger));

            _next = next;
            _logger = logger;
        }

        readonly RequestDelegate _next;
        readonly ILogger<ExceptionMiddleware> _logger;

        public async Task Invoke(HttpContext context) {
            try {
                await _next(context);

            } catch(ValidationException e) {
                const string cause = "Validation";

                _logger.LogWarning(e, cause);

                await HandleServerError(context, HttpStatusCode.BadRequest, new {
                    cause,
                    errors = e.Errors.Select(it => it.ErrorMessage).ToArray()
                });

            } catch(DomainException e) {
                const string cause = "Domain Logic";

                _logger.LogWarning(e, cause);

                await HandleServerError(context, HttpStatusCode.BadRequest, new {
                    cause,
                    errors = new[] { e.Message }
                });
            }
        }

        async Task HandleServerError(HttpContext context, HttpStatusCode status, object exceptionResponse) {
            context.Response.StatusCode = (int)status;
            context.Response.ContentType = "application/json";
            string exceptionJson = JsonConvert.SerializeObject(exceptionResponse);
            await context.Response.WriteAsync(exceptionJson);
        }
    }
}