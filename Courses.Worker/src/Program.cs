using System.Threading.Tasks;
using Courses.Domain;
using Courses.Infrastructure;
using Courses.Utils;
using Courses.Worker.Commands;
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
                        .ForSingletonOf<DbContext>()
                        .Use(new DbContext(connectionString, "courses_dev"));

                    registry
                        .For<ICourseRepository>()
                        .Use<CourseRepository>();

                    registry
                        .ForSingletonOf<IHostedService>()
                        .Use<WorkerService>();

                    registry
                        .For<IMessageReceiver>()
                        .Add(new MessageReceiver(Queues.LogIn))
                        .Named(Queues.LogIn);

                    registry
                        .For<IMessageSender>()
                        .Add(new MessageSender(Queues.Notify))
                        .Named(Queues.Notify);

                    registry
                        .ForConcreteType<WorkerService>()
                        .Configure
                        .Ctor<MessageReceiver>()
                        .Named(Queues.LogIn);

                    registry
                        .ForConcreteType<StudentLogInCommand>()
                        .Configure
                        .Ctor<MessageSender>("messageSender")
                        .Named(Queues.Notify);
                })
                .RunConsoleAsync();
        }
    }
}
