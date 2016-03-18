﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// Start of any command chain / some kind of dummy
    /// </summary>
    internal class RootNode : CommandChainNode
    {
        public RootNode(CommandChainNode parent)
            : base(parent)
        {

        }

        public string Version
        {
            get;
            set;
        }
    }
}
