using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Code fragment types
    /// </summary>
    public enum CodeFragmentType
    {
        /// <summary>
        /// General
        /// </summary>
        None = 0,

        /// <summary>
        /// Defines, that the code fragment only can contains a boolean expression
        /// </summary>
        BooleanExpress = 5,

        /// <summary>
        /// Defines, that the code fragment only can contains executable code
        /// </summary>
        Command = 7
    }
}
