using System;
using System.IO;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Courses.Api {
    public class Program {
        public static int Main(string[] args) {
            IWebHost webHost = BuildWebHost(args);

            var configuration = (IConfiguration)webHost.Services.GetService(typeof(IConfiguration));

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.ColoredConsole()
                .CreateLogger();

            try {
                Log.Information("Courses.Api service is starting...");
                webHost.Run();
                return 0;

            } catch(Exception e) {
                Log.Fatal(e, "Courses.Api service terminated unexpectedly");
                return 1;

            } finally {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>{
                    var env = hostingContext.HostingEnvironment;

                    var sharedFolder = Path.Combine(env.ContentRootPath, "..", "Shared");

                    const string app = "appsettings";
                    const string shared = "sharedSettings";

                    config
                        .AddJsonFile(Path.Combine(sharedFolder, $"{shared}.json"), optional: true)
                        .AddJsonFile(Path.Combine(sharedFolder, $"{shared}.{env.EnvironmentName}.json"), optional: true)
                        .AddJsonFile($"{app}.json", optional: true)
                        .AddJsonFile($"{app}.{env.EnvironmentName}.json", optional: true);

                    config.AddEnvironmentVariables();
                })
                .UseStartup<Startup>()
                .UseLamar()
                .UseSerilog()
                .Build();
    }
}
