using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class ColumnNode : SyntaxTreeNode
    {
        public ColumnNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.Column, token)
        {
            ColumnName = token.Content;
        }

        public override string DebugText
        {
            get
            {
                if (Owner != null)
                {
                    return String.Format("Column <{0}>.<{1}>", Owner, ColumnName ?? "");
                }
                else
                {
                    return String.Format("Column <{0}>", ColumnName ?? "");
                }
            }
        }

        public override void CheckSemantic()
        {

        }

        public string ColumnName
        {
            get;
            private set;
        }

        public string Owner
        {
            get;
            internal set;
        }
    }
}
