using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Interface which must be implemented by all SyntaxTreeNodes which are used for scoping declaration
    /// </summary>
    public interface IScopeNode
    {
        // List with all variables
        SymbolTable SymbolTable { get; }
    }
}
