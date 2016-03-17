using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// List of every syntax node type
    /// </summary>
    public enum SyntaxNodeType
    {
        Dummy = -1,

        EntryPoint = 0,
        Constant = 1,
        Parenthesis = 2,
        Operator = 3,

        Parameter = 99,

        Select = 100,
        Column = 101,
        AllColumn = 102,
        Table = 103,
        Distinct = 104,
        In = 105,
        As = 106,
        First = 107,
        ReturnValueList = 110,
        From = 120,
        Where = 130,

        Insert = 200,
        Into = 201,
        Values = 202,

        FuncCall = 500,
        Arguments = 501
    }
}
