using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class SelectNode : SyntaxTreeNode, IScopeNode
    {
        public SelectNode(SyntaxTreeNode parentNode, RawToken token, SymbolTable parentTable) 
            : base(parentNode, SyntaxNodeType.Select, token)
        {
            SymbolTable = new SymbolTable(parentTable);
            SelectScalar = true;
        }

        public override string DebugText
        {
            get
            {
                return "Select";
            }
        }

        public SymbolTable SymbolTable
        {
            get;
            private set;
        }

        /// <summary>
        /// Select scalar values
        /// </summary>
        public bool SelectScalar
        {
            get;
            set;
        }

        public override void CheckSemantic()
        {

        }
    }
}
