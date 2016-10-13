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
    public class PopToNextColumnCCNode : CommandChainNode
    {
        public PopToNextColumnCCNode(CommandChainNode parent)
            : base(parent)
        {
            
        }

        /// <summary>
        /// Column name/alias
        /// </summary>
        public string ColumnName
        {
            get;
            set;
        }
    }
}
