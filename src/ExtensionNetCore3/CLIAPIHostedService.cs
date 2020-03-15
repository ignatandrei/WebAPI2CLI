using CLIExecute;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("CLITests")]
namespace ExtensionNetCore3
{

    /// <summary>
    /// The service that starts execution of the CLI
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostedService" />
    public class CLIAPIHostedService : IHostedService
    {
        private readonly IApiDescriptionGroupCollectionProvider api;
        private readonly IConfiguration configuration;
        private IServerAddressesFeature serverAddresses;
        /// <summary>The application builder.
        /// It is passed when UseCLI
        /// </summary>
        public IApplicationBuilder app;
        private Timer _timer;
        /// <summary>
        /// Initializes a new instance of the <see cref="CLIAPIHostedService"/> class.
        /// </summary>
        /// <param name="api">The API for find the WebAPI</param>
        /// <param name="configuration">The configuration to find CLI commands</param>
        public CLIAPIHostedService(IApiDescriptionGroupCollectionProvider api, IConfiguration configuration)
        {
            this.api = api;
            this.configuration = configuration;            
        }
        /// <summary>
        /// Determines whether this instance is enabled.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled()
        {
            return configuration.GetValue<int>("CLI_ENABLED") == 1;
        }
        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// It works after server addresses is available
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (IsEnabled())
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            }

            return Task.CompletedTask;

        }
        /// <summary>
        /// Default: false
        /// If true , executes and then not exits.
        /// If false , executes and then exits.
        /// </summary>
        /// <returns>CLI_STAY ==1</returns>
        public bool ShouldStay()
        {
            return configuration.GetValue<int>("CLI_STAY") == 1;
        }
        /// <summary>
        /// Waits to see if the app exists. 
        /// It is passed later in the lifecycle
        /// </summary>
        /// <returns></returns>
        public bool ExistsApp()
        {
            return (app != null);

        }
        private async void DoWork(object state)
        {
            if (!ExistsApp())
            {
                Console.WriteLine("WebAPI2CLI: waiting to have app");
                return;
            }


            serverAddresses = app.ServerFeatures.Get<IServerAddressesFeature>();
            if (serverAddresses == null)
            {
                Console.WriteLine("WebAPI2CLI: waiting to have server adresses");
                return;
            }

            _timer.Dispose();
            exec = new Executor(configuration, serverAddresses, api, app.ApplicationServices);
            await exec.Execute();

            if (!ShouldStay())
                Environment.Exit(0);


        }
        internal Executor exec;
        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        
    }
}
