using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class FromNode : SyntaxTreeNode
    {
        public FromNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.From, token)
        {

        }

        public override string DebugText
        {
            get
            {
                return "From";
            }
        }

        public override void CheckSemantic()
        {

        }
    }
}
