using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    public class CallFunctionCCNode : CommandChainNode
    {
        public CallFunctionCCNode(CommandChainNode parent)
            : base(parent)
        {
            Arugments = new List<string>();
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

        public IList<string> Arugments
        {
            get;
            set;
        }
    }
}
