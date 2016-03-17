using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class DistinctNode : SyntaxTreeNode
    {
        public DistinctNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.Distinct, token)
        {

        }

        public override string DebugText
        {
            get
            {
                return "Distinct";
            }
        }

        public override void CheckSemantic()
        {

        }
    }
}
