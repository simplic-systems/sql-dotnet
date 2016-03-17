using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Result of a parsed-parameter
    /// </summary>
    internal class QuotedParameterParserResult
    {
        /// <summary>
        /// result string, like "Hello World"
        /// </summary>
        public string Result
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the difference of the removed chars, like: "Hello \" Word" will return 1
        /// </summary>
        public int RemovedChars
        {
            get;
            set;
        }
    }
}
