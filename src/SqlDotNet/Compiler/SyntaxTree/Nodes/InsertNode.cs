using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class InsertNode : SyntaxTreeNode, IScopeNode
    {
        public InsertNode(SyntaxTreeNode parentNode, RawToken token, SymbolTable parentTable) 
            : base(parentNode, SyntaxNodeType.Insert, token)
        {
            SymbolTable = new SymbolTable(parentTable);
        }

        public override string DebugText
        {
            get
            {
                return "Insert";
            }
        }

        public SymbolTable SymbolTable
        {
            get;
            private set;
        }

        public override void CheckSemantic()
        {

        }
    }
}
