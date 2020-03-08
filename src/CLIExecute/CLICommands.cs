using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLIExecute
{
    public class CLI_Commands
    {

        private List<CLICommand_v1> Commands_v1 { get; set; } = new List<CLICommand_v1>();

        public CLICommand_v1[] V1
        {
            get
            {
                return Commands_v1?.ToArray();
            }
            set
            {
                Commands_v1 = new List<CLICommand_v1>(value);
            }
        }
        public ICLICommand[] FindCommands(string name)
        {
            var names = name
                .Split(',')
                .Where(it => !string.IsNullOrWhiteSpace(it))
                .Select(it => it.Trim().ToLowerInvariant())
                .ToArray();
            var cmd = V1
                .Where(it =>names.Contains(it.NameCommand.Trim().ToLowerInvariant()))
                .ToArray();

            if(cmd.Length != names.Length)
            {
                //TODO: find names of command that are spelled wrong ;-)
                Console.WriteLine($"there are {names.Length-cmd.Length} commands names not found");
            }

            if (cmd.Length==0)
            {
                throw new ArgumentException($"command {name} not found", name);
            }
            return cmd;
        }
        public CLI_Commands AddCommand(ICLICommand cmd)
        {
            if (cmd is CLICommand_v1 v)
            {
                Commands_v1.Add(v);
                return this;
            }

            throw new ArgumentException($"unsupported type {cmd?.GetType()}");
        }
        
    }
}
