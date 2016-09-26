using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Lexer configuration
    /// </summary>
    public interface ILexerConstants
    {
        /// <summary>
        /// Seperator tokens
        /// </summary>
        char[] TokenSeperator
        {
            get;
        }

        /// <summary>
        /// Following tokens, like >= out of > and =
        /// </summary>
        string[] FollowingTokens
        {
            get;
        }

        /// <summary>
        /// Complex tokens are tokens which has a specific start and end token, like a string: "Hello World"
        /// </summary>
        char[] ComplexToken
        {
            get;
        }

        /// <summary>
        /// String starting a single line comment
        /// </summary>
        string SingleLineComment
        {
            get;
        }

        /// <summary>
        /// String starting a multiline comment
        /// </summary>
        string StartMultilineComment
        {
            get;
        }

        /// <summary>
        /// String ending a multiline comment
        /// </summary>
        string EndMultilineComment
        {
            get;
        }

        /// <summary>
        /// Escape-Charachter in complex tokens
        /// </summary>
        char[] ComplexTokenEscapeChar
        {
            get;
        }
    }
}
