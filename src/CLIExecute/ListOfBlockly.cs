using System;
using System.Collections;
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
        private Tuple<Type, string>[] Types()
        {
            return this
                .Where(it => it.Params != null)
                .SelectMany(it => it.Params)
                .Select(it => it.Value.type)
                .Distinct()
                .Select(it => Tuple.Create(it, BlocklyTypeTranslator(it)))
                .ToArray();
        }
        internal static  string BlocklyTypeTranslator(Type t)
        {
            if (t == typeof(int))
                return "Number";
            if (t == typeof(string))
                return "String";
            if (t == typeof(bool))
                return "Boolean";

            if (typeof(IEnumerable).IsAssignableFrom(t))
                return "Array";
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
                .Select(it => GenerateBlocklyFromType(it.Item1))
                .Select(it => it.descType)
                .ToArray();

            return string.Join(Environment.NewLine, types);
        }
        internal static  string nameType(Type t)
        {
            return BlocklyTypeTranslator(t) ?? t.FullName.Replace(".", "_");
        }
        /// <summary>
        /// Generates the blocks definition.
        /// </summary>
        /// <returns></returns>
        public string GenerateBlocksDefinition()
        {
            var types = this.Types()
                .Where(it => it.Item2 == null)
                .Select(it => it.Item1)
                .ToArray();
            string blockText = "";
            foreach (var type in types)
            {
                blockText += $@"{Environment.NewLine}
                var blockText_{type.Name} = '<block type=""{nameType(type)}""></block>';
                var block_{type.Name} = Blockly.Xml.textToDom(blockText_{type.Name});
                xmlList.push(block_{type.Name});";
            }
            var strDef = $@"
 var registerValues = function() {{
        var xmlList = [];
        {blockText}
                
return xmlList;
              }}  ";
            return strDef;

        }
        private (string nameType, string descType) GenerateBlocklyFromType(Type t)
        {
            var item = BlocklyTypeTranslator(t);
            if (item != null)
                return (item, null);

            string propsDef = "";
            string prodCode = "";
            foreach (var prop in t.GetProperties())
            {
                if (prop.GetGetMethod() == null)
                    continue;

                propsDef += $@"{Environment.NewLine}
                this.appendValueInput('{prop.Name}')
                        .appendField('{prop.Name}')
                        .setCheck({nameType(prop.PropertyType)});";

                prodCode += $@"{Environment.NewLine}
                obj['{prop.Name}'] = Blockly.JavaScript.valueToCode(block, '{prop.Name}', Blockly.JavaScript.ORDER_ATOMIC);
                ";
            }

            var strDef = $@"
                    Blockly.Blocks['{nameType(t)}'] = {{
                    init: function() {{
                        this.appendDummyInput()
                            .appendField('{t.Name}');
                        {propsDef}
                        this.setOutput(true, '{nameType(t)}');
                            }}  
                    }};";

            var strJS = $@"
                Blockly.JavaScript['{nameType(t)}'] = function(block) {{
                var obj={{}};
                {prodCode}
                var code = JSON.stringify(obj)+'\n';
                
                console.log(code);
                return [code, Blockly.JavaScript.ORDER_NONE];
                }};";
            return (t.FullName, strDef + strJS);
        }
        /// <summary>
        /// Functionses to be generated.
        /// </summary>
        /// <returns></returns>
        public string FunctionsToBeGenerated()
        {
            var allDefs = "";
            foreach (var cmd in this)
            {

                allDefs +=Environment.NewLine+ cmd.FunctionDefinition();
                allDefs += Environment.NewLine + cmd.FunctionJSGenerator();
            }
            return allDefs;
        }
        

    }
}
