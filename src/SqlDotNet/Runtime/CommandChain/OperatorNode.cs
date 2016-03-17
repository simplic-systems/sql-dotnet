using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// Type of operators
    /// </summary>
    public enum OperatorType
    {
        Add = 0,
        Sub = 1,
        Mul = 2,
        Div = 3,

        Equal = 5,
        Unequal = 6,
        Greater = 7,
        Smaller = 8,
        GreaterEqual = 9,
        SmallerEqual = 10,

        And = 11,
        Or = 12

        // Add in and so one
    }

    /// <summary>
    /// Execute operator
    /// </summary>
    internal class OperatorNode : CommandChainNode
    {
        public OperatorNode(CommandChainNode parent)
            : base(parent)
        {

        }

        /// <summary>
        /// Current operator type
        /// </summary>
        public OperatorType OpType
        {
            get;
            set;
        }
    }
}
