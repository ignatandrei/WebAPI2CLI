using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CLIExecute
{
    /// <summary>
    /// generator
    /// </summary>
    [DebuggerDisplay("{NameCommand} {Verb}")]
    public class BlocklyGenerator
    {
        /// <summary>
        /// Gets or sets the name of the controller.
        /// </summary>
        /// <value>
        /// The name of the controller.
        /// </value>
        public string ControllerName { get; set; }
        /// <summary>
        /// Gets or sets the type of the return.
        /// </summary>
        /// <value>
        /// The type of the return.
        /// </value>
        public Type ReturnType { get; set; }
        /// <summary>
        /// Gets or sets the name command.
        /// </summary>
        /// <value>
        /// The name command.
        /// </value>
        public string NameCommand { get; set; }
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host { get; set; }
        /// <summary>
        /// Gets or sets the relative request URL.
        /// </summary>
        /// <value>
        /// The relative request URL.
        /// </value>
        public string RelativeRequestUrl { get; set; }
        /// <summary>
        /// Gets or sets the verb.
        /// </summary>
        /// <value>
        /// The verb.
        /// </value>
        public string Verb { get; set; }
        /// <summary>
        /// Gets or sets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType { get; set; } = "application/json";
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public Dictionary<string, (Type type, BindingSource bs)> Params { get; internal set; }


        internal string nameCommand()
        {
            var nameCommand = NameCommand.Replace("/", "_");
            nameCommand = nameCommand.Replace("{", "_").Replace("}", "_");
            return $"{nameCommand}_{Verb}";

        }
        internal string returnFunction()
        {
            if (ReturnType == typeof(void))
            {
                return @" 
                    this.setPreviousStatement(true, null);
                    this.setNextStatement(true, null);";
            }
            else
            {
                return $"this.setOutput(true, {ListOfBlockly.nameType(ReturnType)});";
            }
        }
        internal string propsDefinitionFunction()
        {
            var strPropsDefinition = "";
            if (Params != null)
                foreach (var param in Params)
                {
                    strPropsDefinition += $@"
                    this.appendValueInput('{param.Key} ')
                    .setCheck('{ListOfBlockly.nameType(param.Value.type)}')
                    .appendField('{param.Key}'); ";

                }
            return strPropsDefinition;
        }
        internal string FunctionDefinition()
        {
            var strPropsDefinition = propsDefinitionFunction();

            var returnType = returnFunction();

            return $@"
                Blockly.Blocks['{nameCommand()}'] = {{
  init: function() {{
    this.appendDummyInput()
        .appendField('{nameCommand()}');
        {strPropsDefinition}
        {returnType}
        }}//init
}};//{NameCommand}
";
        }
        string GenerateGet()
        {
            var str = $@"getXhr('{this.RelativeRequestUrl}')";
            if (this.Params == null)
                return  str;  
            
            foreach(var item in Params)
            {
                str = str.Replace("{" + item.Key + "}", $"'+ obj.Key +'");
            }
            str = str + "'";
            return str;
        }
        internal string FunctionJSGenerator()
        {
            var paramsStr = "";
            if (Params != null)
                foreach (var param in Params)
                {
                    paramsStr += $@"
                    obj['value_{param.Key}'] = Blockly.JavaScript.valueToCode(block, '{param.Key}', Blockly.JavaScript.ORDER_ATOMIC);"; ;
                }
            var returnValue = "";
            if (ReturnType == typeof(void))
            {
                returnValue = " return code;";
            }
            else
            {
                returnValue = " return [code, Blockly.JavaScript.ORDER_NONE];";
            }
            
            return $@"
Blockly.JavaScript['{nameCommand()}'] = function(block) {{
var obj={{}};//{RelativeRequestUrl}
{paramsStr}

var code=JSON.stringify(obj);
code+=`{GenerateGet()}`;
{returnValue}
}};
";
        }
    }
}