using ExtensionNetCore3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TestWebAPISite;
using Xbehave;
using Xunit;

namespace CLITests
{
    public class TestAllServices:
        IClassFixture<WebApplicationFactory<Startup>>
    {
        private Dictionary<bool, WebApplicationFactory<Startup>> _factory=new Dictionary<bool, WebApplicationFactory<Startup>>() ;

        public TestAllServices(WebApplicationFactory<Startup> factoryConfig,WebApplicationFactory<Startup> factoryConfig1)
        {
            _factory.Add(true, factoryConfig);
            _factory.Add(false, ConfigureServices(factoryConfig1));
        }
        private WebApplicationFactory<Startup> ConfigureServices(WebApplicationFactory<Startup> f)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, $"appsettings.json");

            return  f.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddJsonFile(configPath);                    
                });

            });

        }
        [Scenario]
        [Example(true)]
        [Example(false)]
        public void TestService(bool WithConfiguration, WebApplicationFactory<Startup> f, object service)
        {
            $"Given the factory configured {WithConfiguration}".x(() =>
            {
                f = _factory[WithConfiguration];
            });
            $"When asking for the service {typeof(CLIAPIHostedService)}".x(() =>
            {
                service = f.Services.GetService(typeof(CLIAPIHostedService));
            });
            $"Then the service should {WithConfiguration} exists".x(() =>
            {
                if (WithConfiguration)
                {
                    service.Should().BeOfType<CLIAPIHostedService>();
                }
                else
                {
                    service.Should().BeNull();
                }
            });
            if (WithConfiguration)
            {
                $"Then service should be enabled".x(() =>
                {
                    var cliService = service as CLIAPIHostedService;
                    cliService.IsEnabled().Should().BeTrue();
                    cliService.ExistsApp().Should().BeTrue();

                });
            }

        }
    }
}
