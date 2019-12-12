using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

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
                .UseStartup<Startup>()
                .UseLamar()
                .UseSerilog()
                .Build();
    }
}
