using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    public class TableNode : SyntaxTreeNode
    {
        public TableNode(SyntaxTreeNode parentNode, RawToken token) 
            : base(parentNode, SyntaxNodeType.Table, token)
        {
            this.TableName = token.Content;
        }

        public override string DebugText
        {
            get
            {
                string returnValue = TableName;

                if (TableAlias != null)
                {
                    returnValue += " AS " + TableAlias;
                }

                if (Owner != null)
                {
                    returnValue = Owner + "." + returnValue;
                }

                return returnValue;
            }
        }

        public override void CheckSemantic()
        {

        }

        public string TableName
        {
            get;
            private set;
        }

        public string TableAlias
        {
            get;
            set;
        }

        public string Owner
        {
            get;
            set;
        }
    }
}
