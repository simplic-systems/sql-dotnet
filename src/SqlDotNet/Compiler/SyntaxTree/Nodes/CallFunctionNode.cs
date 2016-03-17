using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class CallFunctionNode : SyntaxTreeNode
    {
        public CallFunctionNode(SyntaxTreeNode Parent, RawToken token)
            : base(Parent, SyntaxNodeType.FuncCall, token)
        {

        }

        public override void CheckSemantic()
        {

        }

        /// <summary>
        /// Get debug text
        /// </summary>
        public override string DebugText
        {
            get
            {
                return "CallFunction<" + this.Token.Content + ">";
            }
        }
    }
}
