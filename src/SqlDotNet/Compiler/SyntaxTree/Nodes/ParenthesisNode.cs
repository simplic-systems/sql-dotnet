using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class ParenthesisNode : SyntaxTreeNode
    {
        public ParenthesisNode(SyntaxTreeNode Parent, RawToken token)
            : base(Parent, SyntaxNodeType.Parenthesis, token)
        {

        }

        public override void CheckSemantic()
        {

        }

        public BracketType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Get debug text
        /// </summary>
        public override string DebugText
        {
            get
            {
                return "ParenthesisNode";
            }
        }
    }
}
