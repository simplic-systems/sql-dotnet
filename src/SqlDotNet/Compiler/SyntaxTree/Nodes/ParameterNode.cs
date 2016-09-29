using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class ParameterNode : ConstantNode
    {
        public ParameterNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.Parameter, token)
        {

        }

        public override string DebugText
        {
            get
            {
                return "Parameter-?";
            }
        }
    }
}
