using System;
using System.Threading.Tasks;
using Courses.Utils;
using FluentValidation;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Courses.Notification {
    class Program {
        static async Task<int> Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            var hostBuiler = CreateHostBuilder();

             try {
                Log.Information("Courses.Notifier service is starting...");
                await hostBuiler.RunConsoleAsync();
                return 0;

            } catch(Exception e) {
                Log.Fatal(e, "Courses.Notifier service terminated unexpectedly");
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
                        it.AssemblyContainingType<Courses.Utils.IAssemblyFinder>();
                        it.WithDefaultConventions();
                        it.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                        it.ConnectImplementationsToTypesClosing(typeof(AbstractValidator<>));
                    });

                    registry.For<ServiceFactory>().Use(ctx => ctx.GetInstance);

                    registry.For<IMediator>().Use<Mediator>();

                    registry.For(typeof(IPipelineBehavior<,>)).Use(typeof(Courses.Utils.ValidationBehavior<,>));
                    registry.For(typeof(IPipelineBehavior<,>)).Use(typeof(LoggingBehavior<,>));

                    registry.ForSingletonOf<IHostedService>().Use<NotificationService>();

                    registry.For<IMessageReceiver>()
                        .Add(new MessageReceiver(Queues.Notify)).Named(Queues.LogIn);

                    registry.ForConcreteType<NotificationService>().Configure
                        .Ctor<MessageReceiver>().Named(Queues.LogIn);
                })
                .UseSerilog();

    }
}
