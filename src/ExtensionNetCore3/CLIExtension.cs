using CLIExecute;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace ExtensionNetCore3
{
    /// <summary>
    /// helper extension class
    /// </summary>
    public static class CLIExtension
    {
        static CLIExtension()
        {
            var assName = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine($"Web2APICLI:{assName.Name} version:{assName.Version.ToString()}");

        }
        /// <summary>
        /// Adds blockly class
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        /// <returns></returns>
        public static IServiceCollection AddBlockly(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<EnumerateWebAPIHostedService>();
            serviceCollection.AddHostedService<EnumerateWebAPIHostedService>(p => p.GetService<EnumerateWebAPIHostedService>());

            return serviceCollection;
        }
            /// <summary>
            /// Helper method to be used at 
            ///  public void ConfigureServices
            /// </summary>
            /// <param name="serviceCollection"></param>
            /// <returns></returns>
            public static IServiceCollection AddCLI(this IServiceCollection serviceCollection)
        {
            Console.WriteLine($"Web2APICLI:For more details please refer to about Web2APICLI refer to https://github.com/ignatandrei/WebAPI2CLI");

            serviceCollection.TryAddSingleton<IApiDescriptionGroupCollectionProvider, ApiDescriptionGroupCollectionProvider>();
            serviceCollection.TryAddEnumerable(
                ServiceDescriptor.Transient<IApiDescriptionProvider, DefaultApiDescriptionProvider>());
            

            serviceCollection.AddSingleton<CLIAPIHostedService>();

            serviceCollection.AddHostedService<CLIAPIHostedService>(p => p.GetService<CLIAPIHostedService>());
            serviceCollection.AddTransient<ICLICommand_v1>(sc => new CLICommand_v1());
            return serviceCollection;
        }

        /// <summary>
        /// Helper method to be used at
        /// public void Configure
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCLI(this IApplicationBuilder app)
        {

            var service = app.ApplicationServices.GetService<CLIAPIHostedService>();
            service.app = app;            
            return app;
        }
        /// <summary>
        ///  use blockly
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBlockly(this IApplicationBuilder app)
        {

            var service = app.ApplicationServices.GetService<EnumerateWebAPIHostedService>();
            service.app = app;
            app.Map("/blocklyDefinitions", app =>
            {
                var h = app.ApplicationServices.GetService<EnumerateWebAPIHostedService>();
                app.Run(async context =>
                {
                    var b = h.BlocklyTypesDefinition;
                    if (b != null)
                    {
                        var mem = new Memory<byte>(Encoding.UTF8.GetBytes(b));
                        await context.Response.BodyWriter.WriteAsync(mem);
                    }
                });
            });
            app.Map("/blocklyToolboxDefinitions", app =>
            {
                var h = app.ApplicationServices.GetService<EnumerateWebAPIHostedService>();
                app.Run(async context =>
                {
                    var b = h.BlocklyToolBoxDefinition;
                    if (b != null)
                    {
                        var mem = new Memory<byte>(Encoding.UTF8.GetBytes(b));
                        await context.Response.BodyWriter.WriteAsync(mem);
                    }
                });
            });
            app.Map("/blocklyAPIFunctions", app =>
            {
                var h = app.ApplicationServices.GetService<EnumerateWebAPIHostedService>();
                app.Run(async context =>
                {
                    var b = h.BlocklyAPIFunctions;
                    if (b != null)
                    {
                        var mem = new Memory<byte>(Encoding.UTF8.GetBytes(b));
                        await context.Response.BodyWriter.WriteAsync(mem);
                    }
                });
            });
            return app;
        }
        /// <summary>
        /// Makes the zip of the app to download
        /// </summary>
        /// <param name="endpoints">The endpoints.</param>
        /// <param name="app">App builder.</param>
        public static void MapZip(this IEndpointRouteBuilder endpoints, IApplicationBuilder app)
        {
            //see more at 
            //https://andrewlock.net/converting-a-terminal-middleware-to-endpoint-routing-in-aspnetcore-3/
            endpoints.Map("/zip", async context =>
            {
                var response = context.Response;
                var name = Assembly.GetEntryAssembly().GetName().Name + ".zip";
                response.ContentType = "application/octet-stream";
                var b = GetZip(app.ApplicationServices.GetService<IWebHostEnvironment>());
                //https://github.com/dotnet/aspnetcore/blob/master/src/Mvc/Mvc.Core/src/Infrastructure/FileResultExecutorBase.cs
                var contentDisposition = new Microsoft.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                contentDisposition.SetHttpFileName(name);
                response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
                await response.Body.WriteAsync(b);
                
            });
            
            
        }
        private static Memory<byte> GetZip(IWebHostEnvironment env)
        {
            //var b = new Memory<byte>(Encoding.ASCII.GetBytes($"{env.ContentRootPath}"));
            var firstDir = new DirectoryInfo(env.ContentRootPath);
            var nameLength = firstDir.FullName.Length + 1;
            using var memoryStream = new MemoryStream();
            using var zipToOpen = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);

            

            foreach (FileInfo file in firstDir.RecursiveFilesAndFolders().Where(o => o is FileInfo).Cast<FileInfo>())
            {
                var relPath = file.FullName.Substring(nameLength);
                var readmeEntry = zipToOpen.CreateEntryFromFile(file.FullName, relPath);
            }
            zipToOpen.Dispose();
            var b = new Memory<byte>(memoryStream.ToArray());
            return b;
            
        }

        private static IEnumerable<FileSystemInfo> RecursiveFilesAndFolders(this DirectoryInfo dir)
        {
            foreach (var f in dir.GetFiles())
                yield return f;
            foreach (var d in dir.GetDirectories())
            {
                yield return d;
                foreach (var o in RecursiveFilesAndFolders(d))
                    yield return o;
            }
        }
    }
}
