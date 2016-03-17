using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    internal class StaticFactoryToken : FactoryToken
    {
        public StaticFactoryToken(string tokenStr, TokenType type)
            : base(type)
        {

        }

        public override SyntaxTreeNode GetSyntaxNode(RawToken token)
        {
            throw new NotImplementedException();
        }
    }
}
