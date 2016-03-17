using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class WhereNode : SyntaxTreeNode
    {
        public WhereNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.Where, token)
        {

        }

        public override string DebugText
        {
            get
            {
                return "Where";
            }
        }

        public override void CheckSemantic()
        {

        }
    }
}
