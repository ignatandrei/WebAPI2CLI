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

        public TestAllServices(WebApplicationFactory<Startup> factoryConfig)
        {
            
            
            _factory.Add(false, factoryConfig);
            var factoryConfig1 = ConfigureServices(factoryConfig);
            var b = factoryConfig != factoryConfig1;
            _factory.Add(true, factoryConfig1);
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

            $"Then the service should  exists ".x(() =>
            {
                service.Should().BeOfType<CLIAPIHostedService>();

            });
            $"Then service should be enabled: {WithConfiguration} ".x(() =>
            {
                var cliService = service as CLIAPIHostedService;
                cliService.IsEnabled().Should().Be(WithConfiguration);
               
            });
            $"Then later the app should be transmitted".x(async () =>
            {
                var cliService = service as CLIAPIHostedService;
                await Task.Delay(10 * 1000);
                cliService.ExistsApp().Should().BeTrue();
    
            });

        }
    }
}
