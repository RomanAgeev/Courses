using System;
using System.IO;
using System.Threading.Tasks;
using Courses.Db;
using Courses.Utils;
using FluentValidation;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Courses.Worker {
    class Program {
        static async Task<int> Main(string[] args) {
             try {
                await CreateHostBuilder().RunConsoleAsync();
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
                        it.AssemblyContainingType<Courses.Domain.IAssemblyFinder>();
                        it.AssemblyContainingType<Courses.Db.IAssemblyFinder>();
                        it.AssemblyContainingType<Courses.Utils.IAssemblyFinder>();
                        it.WithDefaultConventions();
                        it.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                        it.ConnectImplementationsToTypesClosing(typeof(AbstractValidator<>));
                    });

                    registry.For<ServiceFactory>().Use(ctx => ctx.GetInstance);

                    registry.For<IMediator>().Use<Mediator>();

                    registry.For(typeof(IPipelineBehavior<,>)).Use(typeof(Courses.Utils.ValidationBehavior<,>));
                    registry.For(typeof(IPipelineBehavior<,>)).Use(typeof(LoggingBehavior<,>));
                    
                    string connectionString = context.Configuration.GetConnectionString("CoursesConnection");

                    registry.ForSingletonOf<DbContext>().Use(new DbContext(connectionString));

                    registry.ForSingletonOf<IHostedService>().Use<WorkerService>();

                    registry.For<IMessageReceiver>().Add(new MessageReceiver(Queues.LogIn)).Named(Queues.LogIn);
                    registry.For<IMessageSender>().Add(new MessageSender(Queues.Notify)).Named(Queues.Notify);

                    registry.ForConcreteType<WorkerService>().Configure
                        .Ctor<MessageReceiver>().Named(Queues.LogIn);

                    registry.ForConcreteType<StudentEnrollCommand>().Configure
                        .Ctor<MessageSender>("messageSender").Named(Queues.Notify);
                })
                .ConfigureLogging((context, logging) => {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.ColoredConsole()
                        .CreateLogger();

                    Log.Information("Courses.Api service is starting...");
                })
                .UseSerilog();
    }
}
