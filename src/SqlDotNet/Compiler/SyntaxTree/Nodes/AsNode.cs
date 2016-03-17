using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class AsNode : SyntaxTreeNode
    {
        public AsNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.As, token)
        {

        }

        public override string DebugText
        {
            get
            {
                return "As: " + Alias ?? "<no>";
            }
        }

        public override void CheckSemantic()
        {

        }

        public string Alias
        {
            get;
            set;
        }
    }
}
