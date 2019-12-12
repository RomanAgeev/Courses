using System;
using System.Threading.Tasks;
using Courses.Infrastructure;
using Courses.Utils;
using FluentValidation;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Courses.Worker {
    class Program {
        static async Task<int> Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            var hostBuiler = CreateHostBuilder();

             try {
                Log.Information("Courses.Api service is starting...");
                await hostBuiler.RunConsoleAsync();
                return 0;

            } catch(Exception e) {
                Log.Fatal(e, "Courses.Api service terminated unexpectedly");
                return 1;

            } finally {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder() =>
            new HostBuilder()
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

                    registry.For<ServiceFactory>().Use(ctx => ctx.GetInstance);

                    registry.For<IMediator>().Use<Mediator>();

                    registry.For(typeof(IPipelineBehavior<,>)).Use(typeof(Courses.Utils.ValidationBehavior<,>));
                    registry.For(typeof(IPipelineBehavior<,>)).Use(typeof(LoggingBehavior<,>));

                    string connectionString = "mongodb://dev:dev@localhost:27017/courses_dev";

                    registry.ForSingletonOf<DbContext>().Use(new DbContext(connectionString, "courses_dev"));

                    registry.ForSingletonOf<IHostedService>().Use<WorkerService>();

                    registry.For<IMessageReceiver>().Add(new MessageReceiver(Queues.LogIn)).Named(Queues.LogIn);
                    registry.For<IMessageSender>().Add(new MessageSender(Queues.Notify)).Named(Queues.Notify);

                    registry.ForConcreteType<WorkerService>().Configure
                        .Ctor<MessageReceiver>().Named(Queues.LogIn);

                    registry.ForConcreteType<StudentEnrollCommand>().Configure
                        .Ctor<MessageSender>("messageSender").Named(Queues.Notify);
                })
                .UseSerilog();
    }
}
