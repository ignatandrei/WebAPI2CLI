using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CLIExecute
{
    /// <summary>
    /// serialize commands
    /// </summary>
    public class CLI_Commandserialize
    {
        //public static string Serialize(CLICommand_v1 v1)
        //{
        //    var serializer = new SerializerBuilder().Build();
        //    return serializer.Serialize(v1);
        //}
        //public static ICLICommand_v1 DeSerializeV1(string v1)
        //{
        //    var deserializer = new DeserializerBuilder()
        //        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        //        .Build();
        //    return deserializer.Deserialize<CLICommand_v1>(v1);
        //}
        /// <summary>
        /// Serializes the specified object in YAML
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static string Serialize(object o)
        {
            var serializer = new SerializerBuilder().Build();
            return serializer.Serialize(o);
        }
        /// <summary>
        /// from YAML to CLI_Commands
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>CLI_Commands</returns>
        public static CLI_Commands DeSerialize(string  text)
        {
            var serializer = new DeserializerBuilder()
              //.WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return serializer.Deserialize<CLI_Commands>(text);

        }
    }
}
