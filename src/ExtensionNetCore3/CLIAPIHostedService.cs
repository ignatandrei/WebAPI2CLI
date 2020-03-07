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

    public class CLIAPIHostedService : IHostedService
    {
        private readonly IApiDescriptionGroupCollectionProvider api;
        private readonly IConfiguration configuration;
        private IServerAddressesFeature serverAddresses;
        public IApplicationBuilder app;
        private Timer _timer;
        public CLIAPIHostedService(IApiDescriptionGroupCollectionProvider api, IConfiguration configuration)
        {
            this.api = api;
            this.configuration = configuration;            
        }        
        public bool IsEnabled()
        {
            return configuration.GetValue<int>("CLI_ENABLED") == 1;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (IsEnabled())
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            }

            return Task.CompletedTask;

        }
        public bool ExistsApp()
        {
            return (app != null);

        }
        private async void DoWork(object state)
        {
            if (ExistsApp() && (app.ServerFeatures.Get<IServerAddressesFeature>()!=null))
            {
                _timer.Dispose();
                serverAddresses = app.ServerFeatures.Get<IServerAddressesFeature>();
                
                exec = new Executor(configuration, serverAddresses, api, app.ApplicationServices);
                await exec.Execute();
                Environment.Exit(0);
            }
        }
        internal Executor exec;
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        
    }
}
