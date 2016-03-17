using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class ArgumentNode : SyntaxTreeNode
    {
        public ArgumentNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.Argument, token)
        {

        }

        public override string DebugText
        {
            get
            {
                return "<Arg>";
            }
        }

        public override void CheckSemantic()
        {

        }
    }
}
