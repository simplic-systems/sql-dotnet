using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class FirstNode : SyntaxTreeNode
    {
        public FirstNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.First, token)
        {

        }

        public override string DebugText
        {
            get
            {
                return "First";
            }
        }

        public override void CheckSemantic()
        {

        }
    }
}
