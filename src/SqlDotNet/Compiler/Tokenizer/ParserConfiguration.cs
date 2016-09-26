using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Contains all parser, tokenizer configurations
    /// </summary>
    public class ParserConfiguration : ILexerConstants
    {
        #region Internal constants
        /// <summary>
        /// List of chars, with wich a function could start
        /// </summary>
        public static readonly char[] FunctionBeginParameterChars = new char[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '_'
        };

        /// <summary>
        /// Chars which are allowed in function or parameter names
        /// </summary>
        public static readonly char[] FunctionParameterChars = new char[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',            
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '_', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        /// <summary>
        /// Chars, which are part of <seealso cref="FunctionParameterChars"/> but not allowed at the beginning of a function or parameter
        /// </summary>
        public static readonly char[] NotBeginParameterChars = new char[] 
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };
        #endregion

        #region Public Methods
        /// <summary>
        /// Proof wether the input string is a Int64
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>True if only countains digits</returns>
        public bool IsInt64(string input)
        {
            long dummy = 0;
            return Int64.TryParse(input, out dummy);
        }

        /// <summary>
        /// Proof wether the input string is a Int32
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>True if only countains digits</returns>
        public bool IsInt32(string input)
        {
            int dummy = 0;
            return Int32.TryParse(input, out dummy);
        }

        /// <summary>
        /// Proof, wether the input value is a valid class, function or variable name
        /// </summary>
        /// <param name="value">String to proof</param>
        /// <returns>True if is valid</returns>
        public bool IsValidLanguageIndependentIdentifier(string value)
        {
            return System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(value);
        }

        /// <summary>
        /// Proof wether the input is a string
        /// </summary>
        /// <param name="input">Input string to proof</param>
        /// <returns>True if it is a string</returns>
        public bool IsString(string input)
        {
            return input.StartsWith("'") && input.EndsWith("'");
        }

        /// <summary>
        /// Proof wether the input string is a double or not
        /// </summary>
        /// <param name="input">String to proof</param>
        /// <returns>True if it is a double</returns>
        public bool IsDouble(string input)
        {
            double dummy = 0;
            return ConvertHelper.TryParseDouble(input, out dummy);
        }

        /// <summary>
        /// Proof wether the input string is a float or not
        /// </summary>
        /// <param name="input">String to proof</param>
        /// <returns>True if it is a float</returns>
        public bool IsFloat(string input)
        {
            float dummy = 0;
            return ConvertHelper.TryParseFloat(input, out dummy);
        }

        /// <summary>
        /// Proof wether the input string is a boolean or not
        /// </summary>
        /// <param name="input"String to proof></param>
        /// <returns>True if it is a boolean</returns>
        public bool IsBoolean(string input)
        {
            bool dummy = true;
            return bool.TryParse(input, out dummy);
        }

        /// <summary>
        /// Get the datatype of value
        /// </summary>
        /// <param name="value">Value as strint</param>
        /// <returns>Data type</returns>
        public DataType GetConstDataType(RawToken token)
        {
            DataType returnValue = DataType.None;
            string value = token.Content;

            if (value == "null")
            {
                returnValue = DataType.Null;
            }
            else if (IsString(value))
            {
                returnValue = DataType.Str;
            }
            else if (IsBoolean(value))
            {
                returnValue = DataType.Boolean;
            }
            else if (value.EndsWith("i") && IsInt64(value.Substring(0, value.Length - 1)))
            {
                returnValue = DataType.Int32;
            }
            else if (value.EndsWith("l") && IsInt64(value.Substring(0, value.Length - 1)))
            {
                returnValue = DataType.Int64;
            }
            else if (value.EndsWith("f") && IsDouble(value.Substring(0, value.Length - 1)))
            {
                returnValue = DataType.Float32;
            }
            else if (value.EndsWith("d") && IsDouble(value.Substring(0, value.Length - 1)))
            {
                returnValue = DataType.Float64;
            }
            else if (IsInt32(value))
            {
                returnValue = DataType.Int32;
            }
            else if (IsFloat(value))
            {
                returnValue = DataType.Float32;
            }

            return returnValue;
        }
        #endregion

        #region Public Member
        /// <summary>
        /// Seperator tokens
        /// </summary>
        public char[] TokenSeperator
        {
            get { return new char[] { ' ', '+', '-', '*', '/', '^', '.', '[', ']', ':', '>', '<', '=', '!', '{', '}', '\r', '\n', '(', ')', ',', ';' }; }
        }

        /// <summary>
        /// Following tokens, like >= out of > and =
        /// </summary>
        public string[] FollowingTokens
        {
            get { return new string[] { ">=", "<=", "==", "!=", "\r\n", "/*", "*/", "//", "::", "--" }; }
        }

        /// <summary>
        /// Complex tokens are tokens which has a specific start and end token, like a string: "Hello World"
        /// </summary>
        public char[] ComplexToken
        {
            get { return new char[] { '"', '\'' }; }
        }

        /// <summary>
        /// Escape-Charachter in complex tokens
        /// </summary>
        public char[] ComplexTokenEscapeChar
        {
            get { return new char[] { '\\', '\'' }; }
        }

        /// <summary>
        /// String starting a single line comment
        /// </summary>
        public string SingleLineComment
        {
            get { return "--"; }
        }

        /// <summary>
        /// String starting a multiline comment
        /// </summary>
        public string StartMultilineComment
        {
            get { return "/*"; }
        }

        /// <summary>
        /// String ending a multiline comment
        /// </summary>
        public string EndMultilineComment
        {
            get { return "*/"; }
        }
        #endregion
    }
}
