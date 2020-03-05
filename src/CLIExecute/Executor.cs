using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CLIExecute
{
    public class Executor
    {
        static Executor()
        {
            var assName = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine($"{assName.Name} version:{assName.Version.ToString()}");
        }
        private readonly IServerAddressesFeature serverAddresses;
        private readonly IApiDescriptionGroupCollectionProvider api;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;
        public Executor(IConfiguration configuration, IServerAddressesFeature serverAddresses,IApiDescriptionGroupCollectionProvider api, IServiceProvider serviceProvider)
        {
            this.serverAddresses = serverAddresses;
            this.api = api;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
        }
        
        private string CommandsToExecute()
        {
            return configuration.GetValue<string>("CLICommands");
        }
        public bool ShouldExecuteCommands()
        {
            return !string.IsNullOrWhiteSpace(CommandsToExecute());
        }
        public async Task ExecuteCommands()
        {
            string nameFile = "cli.txt";
            if (!File.Exists(nameFile))
                throw new  FileNotFoundException(nameFile);

            var fileContents = File.ReadAllText(nameFile).Trim();
            var s = CLICommandSerialize.DeSerialize(fileContents);
            var cmds = s.FindCommands(CommandsToExecute());
            foreach (var cmd in cmds)
                if (cmd is ICLICommand_v1 v1)
                {
                    v1.SetPossibleFullHosts(serverAddresses.Addresses.ToArray());
                    try
                    {
                        Console.WriteLine($"executing {v1.NameCommand}");
                        var x = await v1.Execute();
                        Console.WriteLine(JsonSerializer.Serialize(x, new JsonSerializerOptions()
                        {
                            WriteIndented = true
                        }));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);

                    }
                }
        }
        public async Task Execute()
        {
            if (ShouldShowHelp())
            {
                Console.WriteLine("put this in a cli.txt file in the same path as the exe:");
                Console.WriteLine("");
                EnumerateWebAPI();
                Console.WriteLine("");
                Console.WriteLine("then run the exe with following parameters :");
                Console.WriteLine("--CLI_ENABLED=1 --CLICommands=\"Command001,Command002,...\"");
                Console.WriteLine("");

            }

            if (ShouldExecuteCommands())
                await ExecuteCommands();
        }
        public bool ShouldShowHelp()
        {
            var showHelp = configuration.GetValue<int?>("CLI_HELP", null);
            return (showHelp == null || showHelp.Value == 1);
        }
        private void EnumerateWebAPI()
        {
            var allAdresses = serverAddresses.Addresses.ToArray();
            var nr = 0;
            
            var groups = api.ApiDescriptionGroups;
            var allCommands = new CLICommands();
            foreach (var g in groups.Items)
            {
                
                foreach (var api in g.Items)
                {
                    
                    foreach (var adress in allAdresses)
                    {
                        var v1 = serviceProvider.GetService(typeof(ICLICommand_v1))as ICLICommand_v1;

                        v1.NameCommand = $"Command{(++nr).ToString("00#")}";
                        //v1.Host = new Uri(adress).GetLeftPart(UriPartial.Authority);
                        v1.Host = new Uri(adress).GetLeftPart(UriPartial.Scheme);
                        v1.RelativeRequestUrl = api.RelativePath;
                        v1.Verb = api.HttpMethod;
                        v1.DataToSend = GetJsonFromParameters(api.ParameterDescriptions.ToArray());
                        allCommands.AddCommand(v1);
                
                    }
                }
            }
            var serialize = CLICommandSerialize.Serialize(allCommands);
            Console.WriteLine(serialize);
        }
        private string GetJsonFromParameters(ApiParameterDescription[] parameterDescriptions)
        {
            if (parameterDescriptions?.Length == 0)
                return null;

            var desc = new Dictionary<string, (string value, ParameterDescriptor pd)>();
            var pdAll = parameterDescriptions
                .Where(it => it != null)
                .Select(it => it.ParameterDescriptor)
                .Where(it => it != null)
                .Distinct()
                .ToArray();
            var strType = typeof(string).FullName;
            foreach (var pd in pdAll)
            {
                var strValue = "";

                switch (pd.ParameterType)
                {
                    case Type T when T.FullName == strType:
                        strValue = "\"\"";
                        
                        break;
                    case Type T when T.IsValueType:
                        strValue = GetDefaultValue(T).ToString();
                        break;

                    case Type T:
                        var o = FormatterServices.GetUninitializedObject(T);
                        strValue= JsonSerializer.Serialize(o);                        
                        break;
                }
                desc.Add(pd.Name, (strValue, pd));
            }
            var okBindingSource = new[]
            {
                BindingSource.Body,
                BindingSource.Form
            };
            var code =
                string.Join(",",
                desc
                .Where(pd =>
                    pd.Value.pd?.BindingInfo?.BindingSource != null &&
                    okBindingSource.Contains(pd.Value.pd.BindingInfo.BindingSource))
                .Select(it => $"{it.Value.value}")
                .ToArray());
         
            return code;
        }
        object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);
            if (t.FullName == typeof(string).FullName)
            {
                return default(string);
            }

            return null;
        }
    }
}
