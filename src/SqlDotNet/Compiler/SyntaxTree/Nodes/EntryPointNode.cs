using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class EntryPointNode : SyntaxTreeNode, IScopeNode
    {
        public EntryPointNode()
            : base(null, SyntaxNodeType.EntryPoint, null)
        {
            SymbolTable = new SymbolTable(null);
            SelectNodes = new List<SelectNode>();
        }

        public override void CheckSemantic()
        {

        }

        /// <summary>
        /// All select nodes in a query
        /// </summary>
        public IList<SelectNode> SelectNodes
        {
            get;
            private set;
        }

        /// <summary>
        /// Get debug text
        /// </summary>
        public override string DebugText
        {
            get
            {
                return "EntryPoint";
            }
        }

        public SymbolTable SymbolTable
        {
            get;
            private set;
        }
    }
}