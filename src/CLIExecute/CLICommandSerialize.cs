using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CLIExecute
{
    public class CLICommandSerialize
    {
        public static string Serialize(CLICommand_v1 v1)
        {
            var serializer = new SerializerBuilder().Build();
            return serializer.Serialize(v1);
        }
        public static ICLICommand_v1 DeSerializeV1(string v1)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<CLICommand_v1>(v1);
        }
        public static string Serialize(object o)
        {
            var serializer = new SerializerBuilder().Build();
            return serializer.Serialize(o);
        }
        public static CLICommands DeSerialize(string  text)
        {
            var serializer = new DeserializerBuilder()
              //.WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return serializer.Deserialize<CLICommands>(text);

        }
    }
}
