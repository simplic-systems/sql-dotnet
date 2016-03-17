using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class ArgumentList : SyntaxTreeNode
    {
        public ArgumentList(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.Arguments, token)
        {

        }

        public override string DebugText
        {
            get
            {
                return "ArgumentList";
            }
        }

        public override void CheckSemantic()
        {

        }
    }
}
