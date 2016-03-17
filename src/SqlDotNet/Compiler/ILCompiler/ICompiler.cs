using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Compiler interface
    /// </summary>
    public interface ICompiler
    {
        /// <summary>
        /// Compile method impl.
        /// </summary>
        /// <param name="node">Node instance (for exmaple EtnryPointNode)</param>
        /// <returns>Stream with the compiled code</returns>
        CompiledQuery Compile(EntryPointNode node);
    }
}
