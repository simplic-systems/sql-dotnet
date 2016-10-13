using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class ConstantNode : SyntaxTreeNode
    {
        public ConstantNode(SyntaxTreeNode Parent, RawToken token)
            : base(Parent, SyntaxNodeType.Constant, token)
        {

        }

        protected ConstantNode(SyntaxTreeNode Parent, SyntaxNodeType nodeType, RawToken token)
            : base(Parent, nodeType, token)
        {

        }

        public override void CheckSemantic()
        {
            // Proof wether element has children
            if (this.Children.Count != 0)
            {
                throw new Exception("Syntax error near constant.");
            }
        }

        /// <summary>
        /// Get debug text
        /// </summary>
        public override string DebugText
        {
            get
            {
                return "Constant (" + this.Token.Content + "@" + DataType.ToString() + ")";
            }
        }

        public DataType DataType
        {
            get;
            set;
        }
    }
}
