using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Only for internal IL-Compiler usage
    /// </summary>
    internal class DummyNode : SyntaxTreeNode
    {
        /// <summary>
        /// Crate and add child, because thats the only use
        /// </summary>
        /// <param name="child"></param>
        public DummyNode(SyntaxTreeNode child) : base(null, SyntaxNodeType.Dummy, null)
        {
            Children.Enqueue(child);
        }

        public override string DebugText
        {
            get
            {
                return "";
            }
        }

        public override void CheckSemantic()
        {
            
        }
    }
}
