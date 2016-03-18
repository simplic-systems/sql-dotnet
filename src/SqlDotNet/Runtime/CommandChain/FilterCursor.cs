using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// Start of any command chain / some kind of dummy
    /// </summary>
    internal class FilterCursor : CommandChainNode
    {
        public FilterCursor(CommandChainNode parent)
            : base(parent)
        {

        }
    }
}
