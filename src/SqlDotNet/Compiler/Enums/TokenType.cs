using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Contains all token types
    /// </summary>
    public enum TokenType
    {
        Unkown = 0,
        Dot = 1,
        Semicolon = 2,
        Comma = 3,
        NewLine = 4,
        Colon = 5,
        DoubleColon = 6,

        OpenBracket = 20,
        CloseBracket = 21,
        OpenParenthesis = 22,
        CloseParenthesis = 23,
        OpenBraceToken = 24,
        CloseBraceToken = 25,

        Add = 30,
        Subtract = 31,
        Multiply = 32,
        Devide = 33,
        Assign = 34,
        Star = 35,

        EqualOrSet = 40,
        Unequal = 41,
        Greater = 42,
        Smaller = 43,
        GreaterEqual = 44,
        SmallerEqual = 45,

        And = 46,
        Or = 47,
        Not = 48,

        Constant = 400,
        Parameter = 401,
        Name = 500,

        Select = 1000,
        From = 1002,
        Where = 1003,
        Distinct = 1004,
        In = 1005,
        As = 1006,
        Owner = 1007,
        First = 1008,
        Insert = 2000,
        Into = 2001,
        Values = 2002
    }

    /// <summary>
    ///  Defines the types of brackets
    /// </summary>
    public enum BracketType
    {
        Open = 0,
        Close = 1
    }

    public enum BracketProcessType
    { 
        CreateArray = 0,
        AccessIndex = 1
    }
}
