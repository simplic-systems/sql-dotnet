using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public abstract class Symbol
    {
        public SyntaxTreeNode TreeNode
        {
            get;
            set;
        }
    }
}
