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
        internal static string BlocklyTypeBlocks(Type t)
        {
            if (t == typeof(int))
                return "math_number";

            if (t == typeof(string))
                return "text";
            if (t == typeof(bool))
                return "logic_boolean";

            if (typeof(IEnumerable).IsAssignableFrom(t))
                return "lists_create_with";
            //what to do with Array ?
            return null;
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
        public string GenerateBlocksValueDefinition()
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
                
                //console.log(code);
                return [code, Blockly.JavaScript.ORDER_NONE];
                }};";
            return (t.FullName, strDef + strJS);
        }
        /// <summary>
        /// Functions blocklyAPIFunctions to be generated.
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
        internal string GenerateBlocksFunctionsDefinition()
        {
            string blockText = "var blockTextLocalSiteFunctions='';";
            foreach (var cmdAll in this.GroupBy(it => it.ControllerName))
            {
                var key = cmdAll.Key;
                blockText += $"blockTextLocalSiteFunctions += '<category name=\"{key}\">';";
                foreach (var cmd in cmdAll)
                {
                    blockText += $@"{Environment.NewLine}
                        blockTextLocalSiteFunctions += '<block type=""{cmd.nameCommand()}"">';";
                    if(cmd.ExistsParams)
                    foreach(var param in cmd.Params)
                    {
                            var type = param.Value.type;
                        var existing = ListOfBlockly.BlocklyTypeTranslator(type);
                        if(existing != null)
                        {
                                var val = "";
                                if (type.IsValueType)
                                    val = Activator.CreateInstance(type)?.ToString();

                                val = val ?? "";
                                var blockShadow = ListOfBlockly.BlocklyTypeBlocks(param.Value.type);
                                blockText += $@"{Environment.NewLine}
 blockTextLocalSiteFunctions += '<value name=""val_{param.Key}"">';
blockTextLocalSiteFunctions += '<shadow type=""{blockShadow}"">';";
                                switch (blockShadow)
                                {
                                    case "math_number":
                                    blockText += $@"
                                    blockTextLocalSiteFunctions += '<field name=""NUM"">10</field>';
                                    ";
                                        break;
                                    case "text":
                                    
                                        blockText += $@"
                                    blockTextLocalSiteFunctions += '';
                                    blockTextLocalSiteFunctions += '<field name=""TEXT"">abc</field>';
                                    ";
                                        break;
                                    default:
                                        break;
                                }
                                blockText += $@"
 blockTextLocalSiteFunctions += '</shadow></value>';
 ";
                        }
                    }
                    blockText += "blockTextLocalSiteFunctions += '</block>';";
                }
                blockText+=$"blockTextLocalSiteFunctions+='</category>';";
                //blockText += $"blockText_{key} +='</category>';";
                //blockText += $"xmlList.push(Blockly.Xml.textToDom(blockText_{key}));";

            }
            blockText += $"console.log(blockTextLocalSiteFunctions);";

            return blockText;

//            var strDef = $@"
// var registerFunctions = function() {{
//        var xmlList = [];
//        {blockText}
                
//return xmlList;
//              }}  ";
//            return strDef;

        }


    }
}
