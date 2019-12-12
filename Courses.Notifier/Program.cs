using System;
using System.IO;
using System.Threading.Tasks;
using Courses.Utils;
using FluentValidation;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Courses.Notifier {
    class Program {
        static async Task<int> Main(string[] args) {
             try {
                await CreateHostBuilder().RunConsoleAsync();
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
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureHostConfiguration(config => {
                    config.AddEnvironmentVariables(prefix: "DOTNETCORE_");
                })
                .ConfigureAppConfiguration((hostingContext, config) =>{
                    var env = hostingContext.HostingEnvironment;

                    var sharedFolder = Path.Combine(env.ContentRootPath, "..", "Shared");

                    const string shared = "sharedSettings";

                    config
                        .AddJsonFile(Path.Combine(sharedFolder, $"{shared}.json"), optional: true)
                        .AddJsonFile(Path.Combine(sharedFolder, $"{shared}.{env.EnvironmentName}.json"), optional: true);

                    config.AddEnvironmentVariables();
                })
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

                    registry.ForSingletonOf<IHostedService>().Use<NotifierService>();

                    string messagingHost = context.Configuration.GetValue("Messaging:Host", "lolcahost");
                    string queueNotify = context.Configuration.GetValue("Messaging:QueueNotify", "");

                    registry.For<IMessageReceiver>()
                        .Add(new MessageReceiver(messagingHost, queueNotify)).Named(queueNotify);

                    registry.ForConcreteType<NotifierService>().Configure
                        .Ctor<MessageReceiver>().Named(queueNotify);
                })
                .ConfigureLogging((context, logging) => {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.ColoredConsole()
                        .CreateLogger();

                    Log.Information("Courses.Notifier service is starting...");
                })
                .UseSerilog();

    }
}
