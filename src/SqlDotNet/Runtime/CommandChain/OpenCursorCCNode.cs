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
    public class OpenCursorCCNode : CommandChainNode
    {
        public OpenCursorCCNode(CommandChainNode parent)
            : base(parent)
        {

        }

        public string CursorName
        {
            get;
            set;
        }

        public string CursorType
        {
            get;
            set;
        }

        public IList<Schema.ColumnDefinition> Columns
        {
            get;
            set;
        }

        public string CursorSource
        {
            get;
            set;
        }
    }
}
