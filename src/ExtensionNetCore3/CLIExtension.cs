using CLIExecute;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;

namespace ExtensionNetCore3
{
    public static class CLIExtension
    {
        static CLIExtension()
        {
            var assName = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine($"{assName.Name} version:{assName.Version.ToString()}");

        }
        public static IServiceCollection AddCLI(this IServiceCollection serviceCollection)
        {
            Console.WriteLine($"For more details please refer to about Web2APICLI refer to https://github.com/ignatandrei/WebAPI2CLI");

            serviceCollection.TryAddSingleton<IApiDescriptionGroupCollectionProvider, ApiDescriptionGroupCollectionProvider>();
            serviceCollection.TryAddEnumerable(
                ServiceDescriptor.Transient<IApiDescriptionProvider, DefaultApiDescriptionProvider>());
            

            serviceCollection.AddSingleton<CLIAPIHostedService>();

            serviceCollection.AddHostedService<CLIAPIHostedService>(p => p.GetService<CLIAPIHostedService>());
            serviceCollection.AddTransient<ICLICommand_v1>(sc => new CLICommand_v1());
            return serviceCollection;
        }
        public static IApplicationBuilder UseCLI(this IApplicationBuilder app)
        {

            var service = app.ApplicationServices.GetService<CLIAPIHostedService>();
            service.app = app;
            return app;
        }

    }
}
