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
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("CLITests")]
namespace CLIExecute
{
    /// <summary>
    /// Executes multiple commands
    /// </summary>
    public class Executor
    {
        static Executor()
        {
            var assName = Assembly.GetExecutingAssembly().GetName();
            Console.WriteLine($"Web2APICLI:{assName.Name} version:{assName.Version.ToString()}");
        }
        private readonly IServerAddressesFeature serverAddresses;
        private readonly IApiDescriptionGroupCollectionProvider api;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="Executor"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="serverAddresses">The server addresses.</param>
        /// <param name="api">The API.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public Executor(IConfiguration configuration, IServerAddressesFeature serverAddresses,IApiDescriptionGroupCollectionProvider api, IServiceProvider serviceProvider)
        {
            this.serverAddresses = serverAddresses;
            this.api = api;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
        }
        
        private string NameCommandsToExecute()
        {
            //return configuration.GetValue<string>("CLI_Commands");
            return configuration["CLI_Commands"];
        }
        /// <summary>
        /// has some commands to executed in CLI_Commands
        /// </summary>
        /// <returns></returns>
        public bool ShouldExecuteCommands()
        {
            return !string.IsNullOrWhiteSpace(NameCommandsToExecute());
        }
        internal static CLI_Commands FindAllCommands()
        {
            string nameFile = "cli.txt";
            if (!File.Exists(nameFile))
                throw new FileNotFoundException(
                    $"cannot find {nameFile} in {Environment.CurrentDirectory}",nameFile);

            var fileContents = File.ReadAllText(nameFile).Trim();
            var s = CLI_Commandserialize.DeSerialize(fileContents);
            return s;
        }
        /// <summary>
        /// find commands to execute.
        /// </summary>
        /// <returns></returns>
        public ICLICommand[] CommandsToExecute()
        {

            return FindAllCommands().FindCommands(NameCommandsToExecute());
        }
        /// <summary>
        /// Executes the commands.
        /// </summary>
        /// <param name="tw">The writer, console.out</param>
        public async Task ExecuteCommands(TextWriter  tw)
        {
            var cmds = CommandsToExecute();
            foreach (var cmd in cmds)
                if (cmd is ICLICommand_v1 v1)
                {
                    v1.SetPossibleFullHosts(serverAddresses.Addresses.ToArray());
                    try
                    {
                        
                        Console.WriteLine($"Web2APICLI:executing {v1.NameCommand}");
                        var x = await v1.Execute();
                        tw.WriteLine(JsonSerializer.Serialize(x, new JsonSerializerOptions()
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
        /// <summary>
        /// Executes this instance.
        /// </summary>
        public async Task Execute()
        {
            if (ShouldShowHelp())
            {
                Console.WriteLine("Web2APICLI:put this in a cli.txt file in the same path as the exe:");
                Console.WriteLine("");
                var api= EnumerateWebAPI();
                Console.WriteLine(api);
                Console.WriteLine("");
                Console.WriteLine("Web2APICLI:then run the exe with following parameters :");
                Console.WriteLine("--CLI_ENABLED=1 --CLI_Commands=\"Command001,Command002,...\"");
                Console.WriteLine("");
                try
                {
                    File.WriteAllText("cli.txt", api);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Web2APICLI:cannot write cli.txt because " + ex.Message);
                }
            }
            TextWriter sw = Console.Out;
            string nameFile = FileNameToWrite() ;
            bool shouldWrite = !string.IsNullOrEmpty(nameFile);
            if (shouldWrite)
            {
                sw = new StreamWriter(nameFile, true);
            }

            if (ShouldExecuteCommands())
                await ExecuteCommands(sw);

            if (shouldWrite)
                sw.Close();

        }
        /// <summary>
        /// Files the name to write the commands
        /// </summary>
        /// <returns></returns>
        public string FileNameToWrite()
        {
            return configuration["CLI_FILENAME"]; 
        }
        /// <summary>
        /// CLI_HELP ==1  
        /// </summary>
        /// <returns>true/false</returns>
        public bool ShouldShowHelp()
        {
            //deleted for easy of automating testing
            //var showHelp = configuration.GetValue<int?>("CLI_HELP", null);
            var cliHelp = configuration["CLI_HELP"];
            var showHelp = int.TryParse(cliHelp, out var val) && val == 1;
            return showHelp;
        }
        private string EnumerateWebAPI()
        {
            var allAdresses = serverAddresses.Addresses.ToArray();
            var nr = 0;
            
            var groups = api.ApiDescriptionGroups;
            var allCommands = new CLI_Commands();
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
            var serialize = CLI_Commandserialize.Serialize(allCommands);
            return serialize;
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
        internal static object GetDefaultValue(Type t)
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
