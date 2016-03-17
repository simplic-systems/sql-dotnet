using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class IntoNode : SyntaxTreeNode
    {
        public IntoNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.Into, token)
        {

        }

        public override string DebugText
        {
            get
            {
                return "Into";
            }
        }

        public override void CheckSemantic()
        {

        }
    }
}
