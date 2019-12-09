using System.Threading.Tasks;
using Courses.Domain;
using Courses.Infrastructure;
using FluentValidation;
using Lamar.Microsoft.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Courses.Worker {
    class Program {
        static async Task Main(string[] args) {
            await new HostBuilder()
                .UseLamar((context, services) => {
                    services.Scan(it => {
                        it.TheCallingAssembly();
                        it.AssemblyContainingType<Courses.Domain.IAssemblyFinder>();
                        it.AssemblyContainingType<Courses.Infrastructure.IAssemblyFinder>();
                        it.AssemblyContainingType<Courses.Utils.IAssemblyFinder>();
                        it.WithDefaultConventions();
                        it.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                        it.ConnectImplementationsToTypesClosing(typeof(AbstractValidator<>));
                    });

                    services.For<ServiceFactory>().Use(ctx => ctx.GetInstance);
                    services.For<IMediator>().Use<Mediator>();

                    services.For(typeof(IPipelineBehavior<,>)).Use(typeof(Courses.Utils.ValidationBehavior<,>));

                    string connectionString = "mongodb://dev:dev@localhost:27017/courses_dev";

                    services.ForSingletonOf<ICourseRepository>().Use(new CourseRepository(connectionString, "courses_dev"));
                    services.ForSingletonOf<IHostedService>().Use<WorkerService>();    
                })
                .RunConsoleAsync();
        }
    }
}
