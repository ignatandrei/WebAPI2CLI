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
using System.Threading;
using System.Threading.Tasks;

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
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (configuration.GetValue<int>("CLI_ENABLED") == 1)
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            }

            return Task.CompletedTask;

        }
        private async void DoWork(object state)
        {
            bool existsApp = (app != null);            
            if (existsApp)
            {
                _timer.Dispose();
                serverAddresses = app.ServerFeatures.Get<IServerAddressesFeature>();
                var exec = new Executor(configuration, serverAddresses, api, app.ApplicationServices);
                await exec.Execute();
                Environment.Exit(0);
            }
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        
    }
}
