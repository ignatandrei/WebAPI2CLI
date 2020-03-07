using CLIExecute;
using ExtensionNetCore3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestWebAPISite;
using Xbehave;
using Xunit;

namespace CLITests
{
    public class TestMath : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> factoryConfig;

        public TestMath(WebApplicationFactory<Startup> factoryConfig)
        {
            this.factoryConfig = factoryConfig;
        }
        private WebApplicationFactory<Startup> ConfigureServices(WebApplicationFactory<Startup> f, string command)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, $"appsettingsEnableCLI.json");

            return f.WithWebHostBuilder(builder =>
            {
                
                builder.UseSetting("CLI_ENABLED", "1");
                builder.UseSetting("CLICommands", command);
            });

        }
        [Scenario]
        [Example("Test_Get_Add")]
        public void Test(string commandToExecute, WebApplicationFactory<Startup> newFactory, CLIAPIHostedService service)
        {
            $"Given the factory configured".x(() =>
            {
                newFactory = ConfigureServices(this.factoryConfig, commandToExecute);
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
                await Task.Delay(5000);
                var cmds = service.exec?.CommandsToExecute();
                cmds.Should().NotBeNull();
                cmds?.Length.Should().Be(1);
                cmds[0].NameCommand.Should().Be(commandToExecute);

            });
            $"and the execution should be 200".x(async () =>
            {
                var cmds = service.exec?.CommandsToExecute();
                var v1= cmds[0] as ICLICommand_v1;
                var res = await v1.Execute();
                res.StatusCode.Should().Be(HttpStatusCode.OK);

            });

        }
    }
}