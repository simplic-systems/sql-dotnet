using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class ValuesNode : SyntaxTreeNode
    {
        public ValuesNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.Values, token)
        {

        }

        public override string DebugText
        {
            get
            {
                return "Values";
            }
        }

        public override void CheckSemantic()
        {

        }
    }
}
