using Simplic.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Tokenizer, which split the code into single tokens
    /// </summary>
    public class Tokenizer
    {
        #region Private Member
        private Dequeue<RawToken> tokens;
        private ILexerConstants lexerConstants;
        private IErrorListener errorListener;
        #endregion

        #region Constructor
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="lexerConstants">Lexer constants / config</param>
        /// <param name="errorListener">error listener for error capturing</param>
        public Tokenizer(ILexerConstants lexerConstants, IErrorListener errorListener)
        {
            tokens = new Dequeue<RawToken>();
            this.lexerConstants = lexerConstants;
            this.errorListener = errorListener;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Parse script into tokens
        /// </summary>
        /// <param name="code"></param>
        public void ParseAsync(string code)
        {
            string lastToken = "";
            bool lastTokenIsSeperator = false;

            for (int i = 0; i < code.Length; i++)
            {
                char currentChar = code[i];

                // Add Seperators directly as a token
                if (lexerConstants.TokenSeperator.Contains(currentChar))
                {
                    if (lastToken.Trim() != "")
                    {
                        tokens.PushBack(new RawToken(lastToken.Trim(), null, new Tuple<int, int>(i - lastToken.Trim().Length, i)));

                        lastToken = "";
                    }

                    if (currentChar != ' ')
                    {
                        string toEnqueu = currentChar.ToString();

                        // Make from two seperate tokens (> =) one token >=
                        if (lastTokenIsSeperator == true)
                        {
                            if (tokens.Count != 0)
                            {
                                RawToken lastTokenChar = tokens.PeekLast();

                                foreach (string fol in lexerConstants.FollowingTokens)
                                {
                                    if ((lastTokenChar.Content + currentChar) == fol)
                                    {
                                        tokens.PopLast();
                                        toEnqueu = fol;
                                        break;
                                    }
                                }
                            }
                        }

                        // Single-Line comment
                        if (lexerConstants.SingleLineComment == toEnqueu)
                        {
                            string commentString = "";

                            for (i = i - 1; i < code.Length; i++)
                            {
                                commentString += code[i];

                                if (commentString.EndsWith(Environment.NewLine))
                                {
                                    break;
                                }
                            }

                            lastToken = "";
                        }
                        // Multiline comment
                        else if (lexerConstants.StartMultilineComment == toEnqueu)
                        {
                            string commentString = "";

                            bool commentClosed = false;
                            for (i = i - 1; i < code.Length; i++)
                            {
                                commentString += code[i];

                                if (commentString.EndsWith(lexerConstants.EndMultilineComment))
                                {
                                    commentClosed = true;
                                    break;
                                }
                            }

                            // Proof, wether the comment is closed
                            if (commentClosed == false)
                            {
                                errorListener.Report("T0004", "Multiline comment not closed.", i, i - 3, null);
                            }

                            lastToken = "";
                        }
                        else
                        {
                            tokens.PushBack(new RawToken(toEnqueu, null, new Tuple<int, int>((i + 1) - toEnqueu.Trim().Length, (i + 1))));

                            lastTokenIsSeperator = true;

                        }
                    }
                }
                else if (lexerConstants.ComplexToken.Contains(currentChar))
                {
                    if (lastToken.Trim() != "")
                    {
                        tokens.PushBack(new RawToken(lastToken.Trim(), null, new Tuple<int, int>(i - lastToken.Trim().Length, i)));
                        lastToken = "";
                    }

                    // Get Brackets like quoated strings
                    QuotedParameterParserResult result = GetNextComplexString(code.Substring(i, code.Length - i), currentChar, i);

                    i += (result.Result.Length - 1) + result.RemovedChars;

                    tokens.PushBack(new RawToken(result.Result.Trim(), null, new Tuple<int, int>(i - ((result.Result.Length - 1) + result.RemovedChars), (i + 1))));

                    lastTokenIsSeperator = false;
                }
                else if (currentChar == '\t')
                {
                    // Do nothing then
                    if (lastToken.Trim() != "")
                    {
                        tokens.PushBack(new RawToken(lastToken.Trim(), null, new Tuple<int, int>(i - lastToken.Trim().Length, i)));

                        lastToken = "";
                    }
                }
                else
                {
                    lastToken += currentChar;

                    lastTokenIsSeperator = false;
                }
            }

            if (lastToken.Trim().Length > 0)
            {
                tokens.PushBack(new RawToken(lastToken.Trim(), null, new Tuple<int, int>(code.Length - lastToken.Trim().Length, code.Length)));
            }
        }
        #endregion

        #region Private Methods
        private QuotedParameterParserResult GetNextComplexString(string Input, char startEndChar, int startPos)
        {
            // Define return-value / vars
            QuotedParameterParserResult returnValue = new QuotedParameterParserResult();
            returnValue.RemovedChars = 0;
            returnValue.Result = "";

            bool addNextDirect = false;
            int unescapedQuotes = 0;

            for (int i = 0; i < Input.Length; i++)
            {
                if (addNextDirect)
                {
                    returnValue.Result += Input[i];

                    addNextDirect = false;
                    continue;
                }

                // Escape, but only for chars that are unequal to the start end char, for example '\''
                if (lexerConstants.ComplexTokenEscapeChar.Where(item => item != startEndChar).Contains(Input[i]))
                {
                    // This char will not be added to the result
                    returnValue.RemovedChars++;

                    addNextDirect = true;
                    continue;
                }
                // Escape, but only for chars that are EQUAL to the start end char, for example ''''
                // and is not the first or the last character
                if (lexerConstants.ComplexTokenEscapeChar.Where(item => item == startEndChar).Contains(Input[i]) 
                    && i > 0 
                    && i < (Input.Length - 1)
                    && Input[i + 1] == startEndChar)
                {
                    // This char will not be added to the result
                    returnValue.RemovedChars++;

                    addNextDirect = true;
                    continue;
                }

                if (Input[i] == startEndChar)
                {
                    unescapedQuotes++;
                }

                returnValue.Result += Input[i];

                // Leave if all unescaped quoates are closed
                if (unescapedQuotes == 2)
                {
                    returnValue.Result = returnValue.Result.Substring(0, (i - returnValue.RemovedChars) + 1);
                    break;
                }
            }

            if (unescapedQuotes % 2 != 0)
            {
                errorListener.Report("T0002", "Expected close token: " + startEndChar.ToString(), startPos, 1, null);
            }

            return returnValue;
        }
        #endregion

        #region Public Member
        /// <summary>
        /// Tokens from the current tokenizer
        /// </summary>
        public Dequeue<RawToken> Tokens
        {
            get { return tokens; }
            set { tokens = value; }
        }
        #endregion
    }
}
