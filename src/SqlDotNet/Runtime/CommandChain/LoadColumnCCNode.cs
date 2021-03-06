﻿using SqlDotNet.Compiler;
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
    public class LoadColumnCCNode : CommandChainNode
    {
        public LoadColumnCCNode(CommandChainNode parent)
            : base(parent)
        {

        }

        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the cursor
        /// </summary>
        public string Cursor
        {
            get;
            set;
        }
    }
}
