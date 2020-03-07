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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
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
            builder.UseSetting("CLI_ENABLED", "1");
            builder.UseSetting("CLI_STAY", "1");
            //builder.UseSetting("CLICommands", "Test_Get_Add");
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
        static TestMath()
        {
            ServicePointManager.ServerCertificateValidationCallback =
            delegate (
                object s,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors
            )
            {
                return true;
            };
        }
        public TestMath(LocalServerFactory<Startup> factoryConfig)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors
                )
                {
                    return true;
                };

            this.factoryConfig = factoryConfig;
        }
        //private WebApplicationFactory<Startup> ConfigureServices(WebApplicationFactory<Startup> f, string command)
        //{
        //    var projectDir = Directory.GetCurrentDirectory();
        //    var configPath = Path.Combine(projectDir, $"appsettingsEnableCLI.json");
        //    f.ClientOptions.BaseAddress= new Uri("https://localhost:5000");
        //    return f.WithWebHostBuilder(builder =>
        //    {
        //        builder.UseSetting("CLI_STAY", "1");
        //        builder.UseSetting("CLICommands", command);
        //        //builder.Start();
        //    });

        //}
        [Scenario]
        //[Example("Test_Get_Add_Https","[1,2]")]
        [Example("Test_Get_Add_Http", "[1,2]")]
        public void TestCommand(string commandToExecute, string result, CLIAPIHostedService service)
        {
            var newFactory = this.factoryConfig;// ConfigureServices(this.factoryConfig, commandToExecute); ;
            string address = newFactory.RootUri;
            $"Given the starting addresses configured {address} ".x(() =>
            {

            });
            //$"When asking for the service {typeof(CLIAPIHostedService)}".x(() =>
            //{
            //    service = newFactory.Services.GetService(typeof(CLIAPIHostedService)) as CLIAPIHostedService;
            //});

            //$"Then the service should  exists ".x(() =>
            //{

            //    service.Should().NotBeNull();

            //});
            //$"And the executor should execute the command".x(async () =>
            //{
            //    return;
            //    var c = newFactory.CreateClient();
            //    //var r = await c.GetAsync("api/Mathadd");
            //    //r.StatusCode.Should().Be(HttpStatusCode.OK);
            //    await Task.Delay(10000);
            //    var cmds = service.exec?.CommandsToExecute();
            //    cmds.Should().NotBeNull();
            //    cmds?.Length.Should().Be(1);
            //    cmds[0].NameCommand.Should().Be(commandToExecute);

            //});
            CLICommands cmds = null;
            $"When finding  all commands".x(() =>
            {
                cmds = Executor.FindAllCommands();
                cmds.V1.Should().NotBeNull();
                cmds.V1.Length.Should().BeGreaterThan(0);
            });
            $"and the execution of {commandToExecute} should be 200 and the result {result} ".x(async () =>
            {


                var find = cmds.FindCommands(commandToExecute);
                find.Should().NotBeNull();
                find.Length.Should().Be(1);
                var v1 = find[0] as ICLICommand_v1;
                v1.Should().NotBeNull();
                v1.SetPossibleFullHosts(newFactory.RootUri);
                var res = await v1.Execute();
                res.StatusCode.Should().Be(HttpStatusCode.OK);
                res.Result.Should().Be(result);
            });

        }
    }
}