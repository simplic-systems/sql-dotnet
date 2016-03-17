﻿using SqlDotNet.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
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
