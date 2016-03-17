using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class ReturnValueList : SyntaxTreeNode
    {
        public ReturnValueList(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.ReturnValueList, token)
        {

        }

        public override string DebugText
        {
            get
            {
                return "ReturnValueList";
            }
        }

        public override void CheckSemantic()
        {

        }
    }
}
