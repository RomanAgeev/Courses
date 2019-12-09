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

namespace Courses.Api {
    public class Program {
        public static int Main(string[] args) {
            IWebHost webHost = BuildWebHost(args);
            try {
                webHost.Run();
                return 0;
            }
            catch {
                return 1;
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseLamar()
                .Build();
    }
}
