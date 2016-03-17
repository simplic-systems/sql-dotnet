using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class AllColumnNode : SyntaxTreeNode
    {
        public AllColumnNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.AllColumn, token)
        {

        }

        public override string DebugText
        {
            get
            {
                string returnValue = "*";
                if (Owner != null)
                {
                    returnValue += "." + Owner;
                }

                returnValue += ColumnName;

                return returnValue;
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
            set;
        }
    }
}
