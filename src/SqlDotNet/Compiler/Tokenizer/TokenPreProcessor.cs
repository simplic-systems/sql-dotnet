using Simplic.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Preprocessor to clean and order all tokens before creating the syntax tree.
    /// Tokenizer --> TokenPreProcessor --> SyntaxTreeBuilder
    /// </summary>
    public class TokenPreProcessor
    {
        /// <summary>
        /// Process tokens, by cleaning them up and reorder them
        /// </summary>
        /// <param name="tokens">List of unprocessed tokens</param>
        /// <param name="parserConfig">Parser configuration</param>
        /// <param name="errorListener">Optional Error listener for error reporting</param>
        /// <returns>Dequeue of processed tokens</returns>
        public Dequeue<RawToken> Process(Dequeue<RawToken> tokens, ParserConfiguration parserConfig, IErrorListener errorListener = default(IErrorListener))
        {
            Dequeue<RawToken> processedTokens = new Dequeue<RawToken>();

            RawToken lastToken = null;

            // Token post-process
            while (tokens.Count > 0)
            {
                RawToken token = tokens.PopFirst();

                #region [Create decimal number (1 . 2 ==> 1.2)]
                if (token.Content == ".")
                {
                    if (processedTokens.Count > 0 && parserConfig.IsInt64(processedTokens.PeekLast().Content))
                    {
                        if (tokens.Count > 0)
                        {
                            RawToken lastProcssedToken = processedTokens.PopLast();
                            RawToken nextToken = tokens.PopFirst();

                            if (parserConfig.IsInt64(nextToken.Content) == false && nextToken.Content.EndsWith("d") == false && nextToken.Content.EndsWith("f"))
                            {
                                errorListener.Report("T0001", "Syntax error, exptected numeric value: " + nextToken.Content, nextToken.Index.Item1, nextToken.Index.Item2, nextToken);
                            }

                            token = new RawToken(lastProcssedToken.Content + token.Content + nextToken.Content, null, new Tuple<int, int>(lastProcssedToken.Index.Item1, nextToken.Index.Item2));
                        }
                        else
                        {
                            errorListener.Report("T0005", "Syntax error, expected token behind: " + token.Content, token.Index.Item1, token.Index.Item2, token);
                        }
                    }
                }
                #endregion

                #region [Classify tokens]
                // if a token starts or ends with ", it is only because the tokenizer should
                // handle this as one token. So we cam remove it here:

                if (token.Content.StartsWith("\"") && token.Content.EndsWith("\""))
                {
                    token.Content = token.Content.Substring(1, token.Content.Length - 2);
                }

                string tcontent = token.Content;

                var constType = parserConfig.GetConstDataType(token);

                if (token.Content.StartsWith("'") && token.Content.EndsWith("'"))
                {
                    token.Type = TokenType.Constant;
                }
                else if (SyntaxTreeFactory.Singleton.StaticTokens.ContainsKey(tcontent))
                {
                    token.Type = SyntaxTreeFactory.Singleton.StaticTokens[tcontent].Type;
                }
                else if (constType != DataType.None)
                {
                    token.Type = TokenType.Constant;
                }
                else if (parserConfig.IsValidLanguageIndependentIdentifier(tcontent) || tcontent.StartsWith("$") || tcontent.StartsWith("@"))
                {
                    token.Type = TokenType.Name;

                    // Name to Owner, when <owner>.<name> is true
                    if (tokens.Count > 0 && tokens.PeekFirst().Content == ".")
                    {
                        tokens.PopFirst(); // Remove dot, because the owner is already detected
                        token.Type = TokenType.Owner;
                    }
                }
                else
                {
                    errorListener?.Report("T0006", "Invalid token: " + token.Content, token.Index.Item1, token.Index.Item2, token);
                }
                #endregion

                // Proof whether a star/multiply is used for select all columns or just for multiply values
                #region [Multiply/Start]
                if (token.Type == TokenType.Multiply)
                {
                    // Use start instead of multiply for the following scenarios: SELECT * or SELECT a.* or SELECT a, *
                    if (lastToken != null && (lastToken.Type == TokenType.Select || lastToken.Type == TokenType.Comma || lastToken.Type == TokenType.Owner))
                    {
                        token.Type = TokenType.Star;
                    }
                }
                #endregion

                lastToken = token;
                processedTokens.PushBack(token);
            }

            return processedTokens;
        }
    }
}
