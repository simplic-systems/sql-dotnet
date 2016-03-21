using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    public class CallFunctionNode : CommandChainNode
    {
        public CallFunctionNode(CommandChainNode parent)
            : base(parent)
        {

        }

        /// <summary>
        /// Name of the current function
        /// </summary>
        public string FunctionName
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public int ParameterCount
        {
            get;
            set;
        }
    }
}
