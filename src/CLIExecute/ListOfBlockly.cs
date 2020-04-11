using System;
using System.Collections.Generic;
using System.Linq;

namespace CLIExecute
{
    class ListOfBlockly : List<BlocklyGenerator>
    {
        public Type[] Types()
        {
            return this
                .SelectMany(it => it.Params)
                .Select(it => it.Value.type)
                .Distinct()
                .ToArray();
        }

    }
}
