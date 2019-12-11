using System;
using System.Threading.Tasks;
using Courses.Utils;
using FluentValidation;
using Lamar;
using Lamar.Microsoft.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Courses.Notification {
    class Program {
        static async Task Main(string[] args) {
             await new HostBuilder()
                .UseLamar((context, registry) => {
                    registry.Scan(it => {
                        it.TheCallingAssembly();
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

                    registry
                        .ForSingletonOf<IHostedService>()
                        .Use<NotificationService>();

                    registry
                        .For<IMessageReceiver>()
                        .Add(new MessageReceiver(Queues.Notify))
                        .Named(Queues.LogIn);

                    registry
                        .ForConcreteType<NotificationService>()
                        .Configure
                        .Ctor<MessageReceiver>()
                        .Named(Queues.LogIn);
                })
                .RunConsoleAsync();
        }
    }
}
