using SqlDotNet.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// Load a constant value onto the stack
    /// </summary>
    internal class LoadArgumentCCNode : CommandChainNode
    {
        public LoadArgumentCCNode(CommandChainNode parent)
            : base(parent)
        {

        }

        /// <summary>
        /// Order id
        /// </summary>
        public int Id
        {
            get;
            set;
        }
    }
}
