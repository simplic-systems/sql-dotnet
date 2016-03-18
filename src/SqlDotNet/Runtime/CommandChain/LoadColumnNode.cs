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
    internal class LoadColumnNode : CommandChainNode
    {
        public LoadColumnNode(CommandChainNode parent)
            : base(parent)
        {

        }

        /// <summary>
        /// Value of the current constant
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Column owner
        /// </summary>
        public string Owner
        {
            get;
            set;
        }
    }
}
