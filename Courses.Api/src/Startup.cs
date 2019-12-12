using Courses.Api.Commands;
using Courses.Api.Middleware;
using Courses.Db;
using Courses.Utils;
using FluentValidation;
using Lamar;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Api {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureContainer(ServiceRegistry registry) {
            registry
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            registry.Scan(it => {
                it.TheCallingAssembly();
                it.AssemblyContainingType<Courses.Domain.IAssemblyFinder>();
                it.AssemblyContainingType<Courses.Db.IAssemblyFinder>();
                it.AssemblyContainingType<Courses.Utils.IAssemblyFinder>();
                it.WithDefaultConventions();
                it.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                it.ConnectImplementationsToTypesClosing(typeof(AbstractValidator<>));
            });

            string connectionString = Configuration.GetConnectionString("CoursesConnection");

            registry.ForSingletonOf<DbContext>().Use(new DbContext(connectionString));

            registry.For(typeof(AbstractValidator<StudentEnrollCommandV1>)).Use(typeof(StudentEnrollCommandV1.Validator));
            registry.For(typeof(AbstractValidator<StudentEnrollCommandV2>)).Use(typeof(StudentEnrollCommandV2.Validator));

            registry.For<ServiceFactory>().Use(ctx => ctx.GetInstance);

            registry.For<IMediator>().Use<Mediator>();

            registry.For(typeof(IPipelineBehavior<,>)).Use(typeof(ValidationBehavior<,>));
            registry.For(typeof(IPipelineBehavior<,>)).Use(typeof(LoggingBehavior<,>));

            registry.For<IMessageSender>()
                .Add(new MessageSender(Queues.LogIn)).Named(Queues.LogIn);

            registry.ForConcreteType<StudentEnrollCommandHandlerV2>().Configure
                .Ctor<IMessageSender>().Named(Queues.LogIn);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMvc();
        }
    }
}
