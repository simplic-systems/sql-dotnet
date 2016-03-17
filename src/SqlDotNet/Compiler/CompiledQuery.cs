using SqlDotNet.Runtime;
using System;
using System.Collections.Generic;
using System.IO;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// The result of an compiled query, containing results of multiple compiling steps
    /// </summary>
    public class CompiledQuery
    {
        /// <summary>
        /// AST root (entry point of the sql "programm")
        /// </summary>
        public EntryPointNode EntryPoint
        {
            get;
            set;
        }

        /// <summary>
        /// Root-node of the flat command chain
        /// </summary>
        internal CommandChainNode CommandChainRoot
        {
            get;
            set;
        }

        /// <summary>
        /// Compiled IL-Code
        /// </summary>
        public Stream ILCode
        {
            get;
            internal set;
        }
    }
}
