using SqlDotNet.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// Load a parameter value onto the stack
    /// </summary>
    public class LoadParameterCCNode : CommandChainNode
    {
        public LoadParameterCCNode(CommandChainNode parent)
            : base(parent)
        {

        }

        /// <summary>
        /// Index of the parameter
        /// </summary>
        public int Index
        {
            get;
            set;
        }
    }
}
