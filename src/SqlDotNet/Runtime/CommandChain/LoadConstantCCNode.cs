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
    internal class LoadConstantCCNode : CommandChainNode
    {
        public LoadConstantCCNode(CommandChainNode parent)
            : base(parent)
        {

        }

        /// <summary>
        /// Value of the current constant
        /// </summary>
        public object ConstantValue
        {
            get;
            set;
        }

        /// <summary>
        /// Datatype
        /// </summary>
        public DataType DataType
        {
            get;
            set;
        }
    }
}
