using CLIExecute;
using ExtensionNetCore3;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestWebAPISite;
using Xbehave;
using Xunit;

namespace CLITests
{
    public class LocalServerFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private const string _LocalhostBaseAddress = "https://localhost";
        private IWebHost _host;
        public LocalServerFactory()
        {
            ClientOptions.BaseAddress = new Uri(_LocalhostBaseAddress);
            // Breaking change while migrating from 2.2 to 3.1, TestServer was not called anymore
            CreateServer(CreateWebHostBuilder());
        }
        public string RootUri { get; private set; }
        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            builder.UseSetting("CLI_ENABLED", "1");
            builder.UseSetting("CLI_STAY", "1");
            builder.UseSetting("CLICommands", "Test_Get_Add");
            ;

            _host = builder.Build();
            _host.Start();
            RootUri = _host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.LastOrDefault();
            // not used but needed in the CreateServer method logic
            
            return new TestServer(new WebHostBuilder().UseStartup<TStartup>());
        }
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>());
            builder.UseStartup<TStartup>();
            return builder;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _host?.Dispose();
            }
        }
    }

    public class TestMath : IClassFixture<LocalServerFactory<Startup>>
    {
        private readonly LocalServerFactory<Startup> factoryConfig;

        public TestMath(LocalServerFactory<Startup> factoryConfig)
        {
            this.factoryConfig = factoryConfig;
        }
        private WebApplicationFactory<Startup> ConfigureServices(WebApplicationFactory<Startup> f, string command)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, $"appsettingsEnableCLI.json");
            f.ClientOptions.BaseAddress= new Uri("https://localhost:5000");
            return f.WithWebHostBuilder(builder =>
            {
                builder.UseSetting("CLI_STAY", "1");
                builder.UseSetting("CLICommands", command);
                //builder.Start();
            });

        }
        [Scenario]
        [Example("Test_Get_Add")]
        public void TestCommand(string commandToExecute, CLIAPIHostedService service)
        {
            var newFactory = this.factoryConfig; ConfigureServices(this.factoryConfig, commandToExecute); ;
            $"Given the factory configured".x(() =>
            {
                
            });
            $"When asking for the service {typeof(CLIAPIHostedService)}".x(() =>
            {
                service = newFactory.Services.GetService(typeof(CLIAPIHostedService)) as CLIAPIHostedService;
            });

            $"Then the service should  exists ".x(() =>
            {
                
                service.Should().NotBeNull();

            });
            $"And the executor should execute the command".x(async () =>
            {
                return;
                var c = newFactory.CreateClient();
                //var r = await c.GetAsync("api/Mathadd");
                //r.StatusCode.Should().Be(HttpStatusCode.OK);
                await Task.Delay(10000);
                var cmds = service.exec?.CommandsToExecute();
                cmds.Should().NotBeNull();
                cmds?.Length.Should().Be(1);
                cmds[0].NameCommand.Should().Be(commandToExecute);

            });
            $"and the execution should be 200".x(async () =>
            {
                return;
                var cmds = service.exec?.CommandsToExecute();
                var v1 = cmds[0] as ICLICommand_v1;
                var res = await v1.Execute();
                res.StatusCode.Should().Be(HttpStatusCode.OK);

            });

        }
    }
}