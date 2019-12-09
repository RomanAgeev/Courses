using System.Threading.Tasks;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Courses.Worker {
    class Program {
        static async Task Main(string[] args) {
            await new HostBuilder()
                .UseLamar((context, registry) => {
                    registry.ForSingletonOf<IHostedService>().Use<WorkerService>();

                    string connectionString = "localhost:27017";

                })
                .RunConsoleAsync();
        }
    }
}
