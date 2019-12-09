using System.Threading.Tasks;
using Courses.Domain;
using Courses.Infrastructure;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Courses.Worker {
    class Program {
        static async Task Main(string[] args) {
            await new HostBuilder()
                .UseLamar((context, services) => {
                    services.Scan(it => {
                        it.TheCallingAssembly();
                        it.AssemblyContainingType<Courses.Domain.IAssemblyFinder>();
                        it.AssemblyContainingType<Courses.Infrastructure.IAssemblyFinder>();
                        it.WithDefaultConventions();
                    });

                    string connectionString = "mongodb://dev:dev@localhost:27017/courses_dev";

                    services.ForSingletonOf<ICourseRepository>().Use(new CourseRepository(connectionString, "courses_dev"));
                    services.ForSingletonOf<IHostedService>().Use<WorkerService>();

                })
                .RunConsoleAsync();
        }
    }
}
