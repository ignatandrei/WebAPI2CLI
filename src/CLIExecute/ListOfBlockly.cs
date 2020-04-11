using System;
using System.Collections.Generic;
using System.Linq;

namespace CLIExecute
{
    /// <summary>
    /// all blockly that should be generated
    /// </summary>
    /// 
    public class ListOfBlockly : List<BlocklyGenerator>
    {
        /// <summary>
        /// all types
        /// </summary>
        /// <returns></returns>
        private Tuple<Type,string>[] Types()
        {
            return this
                .Where(it=>it.Params != null)
                .SelectMany(it => it.Params)                
                .Select(it => it.Value.type)
                .Distinct()
                .Select(it=> Tuple.Create(it,BlocklyTypeTranslator(it)))
                .ToArray();
        }
        private string BlocklyTypeTranslator(Type t)
        {
            if (t == typeof(int))
                return "Number";
            if (t == typeof(string))
                return "String";
            if (t == typeof(bool))
                return "Boolean";
            //what to do with Array ?
            return null;
        }
        /// <summary>
        /// Generates types of Blockly
        /// </summary>
        /// <returns></returns>
        public string TypesToBeGenerated()
        {
            var types = this.Types()
                .Where(it => it.Item2 == null)
                .Select(it=>GenerateBlocklyFromType(it.Item1))
                .Select(it=>it.descType)
                .ToArray();

            return string.Join(Environment.NewLine, types);
        }
        private string nameType(Type t)
        {
            return BlocklyTypeTranslator(t) ?? t.FullName;
        }
        
        private ( string nameType, string descType) GenerateBlocklyFromType(Type t)
        {
            var item = BlocklyTypeTranslator(t);
            if (item != null)
                return (item, null);

            string props = "";
            foreach(var prop in t.GetProperties())
            {
                if (prop.GetGetMethod() == null)
                    continue;
                
                props += $@"{Environment.NewLine}
                this.appendValueInput('{prop.Name}')
                        .appendField('{prop.Name}')
                        .setCheck({nameType(prop.PropertyType)});";
            }

            var str = $@"
                    Blockly.Blocks['{t.FullName}'] = {{
                    init: function() {{
                        this.appendDummyInput()
                            .appendField('{t.Name}');
                        {props}
                            }}
                    }};";
            return (t.FullName, str);
        }
        

    }
}
