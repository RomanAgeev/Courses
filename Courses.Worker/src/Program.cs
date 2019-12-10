using System.Threading.Tasks;
using Courses.Domain;
using Courses.Infrastructure;
using Courses.Utils;
using FluentValidation;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Courses.Worker {
    class Program {
        static async Task Main(string[] args) {
            await new HostBuilder()
                .UseLamar((context, registry) => {
                    registry.Scan(it => {
                        it.TheCallingAssembly();
                        it.AssemblyContainingType<Courses.Domain.IAssemblyFinder>();
                        it.AssemblyContainingType<Courses.Infrastructure.IAssemblyFinder>();
                        it.AssemblyContainingType<Courses.Utils.IAssemblyFinder>();
                        it.WithDefaultConventions();
                        it.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                        it.ConnectImplementationsToTypesClosing(typeof(AbstractValidator<>));
                    });

                    registry
                        .For<ServiceFactory>()
                        .Use(ctx => ctx.GetInstance);

                    registry
                        .For<IMediator>()
                        .Use<Mediator>();

                    registry
                        .For(typeof(IPipelineBehavior<,>))
                        .Use(typeof(Courses.Utils.ValidationBehavior<,>));

                    string connectionString = "mongodb://dev:dev@localhost:27017/courses_dev";

                    registry
                        .ForSingletonOf<ICourseRepository>()
                        .Use(new CourseRepository(connectionString, "courses_dev"));

                    registry
                        .ForSingletonOf<IHostedService>()
                        .Use<WorkerService>();

                    registry
                        .For<IMessageReceiver>()
                        .Add(new MessageReceiver(Queues.LogIn))
                        .Named(Queues.LogIn);

                    registry
                        .ForConcreteType<WorkerService>()
                        .Configure
                        .Ctor<MessageReceiver>()
                        .Named(Queues.LogIn);
                })
                .RunConsoleAsync();
        }
    }
}
