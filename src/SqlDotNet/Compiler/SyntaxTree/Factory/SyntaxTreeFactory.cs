using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Syntax tree factory has the responsability to create all SyntaxTreeNodes by using Factory Tokens
    /// </summary>
    internal class SyntaxTreeFactory
    {
        #region Singleton
        private static readonly SyntaxTreeFactory singleton = new SyntaxTreeFactory();

        /// <summary>
        /// Singleton access to recude instance creation, because the factory will not change at runtime
        /// </summary>
        internal static SyntaxTreeFactory Singleton
        {
            get { return SyntaxTreeFactory.singleton; }
        }
        #endregion

        #region Private Member
        private IDictionary<string, StaticFactoryToken> staticTokens;
        private IDictionary<TokenType, DynamicFactoryToken> dynamicTokens;
        #endregion

        #region Constructor
        /// <summary>
        /// Create syntact tree factory
        /// </summary>
        private SyntaxTreeFactory()
        {
            // Create list of static factory tokens
            staticTokens = new Dictionary<string, StaticFactoryToken>(StringComparer.OrdinalIgnoreCase);

            // Add all static factory tokens once
            staticTokens.Add(".", new StaticFactoryToken(".", TokenType.Dot));
            staticTokens.Add(";", new StaticFactoryToken(";", TokenType.Semicolon));
            staticTokens.Add(",", new StaticFactoryToken(",", TokenType.Comma));
            staticTokens.Add(":", new StaticFactoryToken(":", TokenType.Colon));
            staticTokens.Add("::", new StaticFactoryToken("::", TokenType.DoubleColon));
            staticTokens.Add(Environment.NewLine, new StaticFactoryToken(Environment.NewLine, TokenType.NewLine));
            staticTokens.Add("\n", new StaticFactoryToken("\n", TokenType.NewLine));

            staticTokens.Add("+", new StaticFactoryToken("+", TokenType.Add));
            staticTokens.Add("-", new StaticFactoryToken("-", TokenType.Subtract));
            staticTokens.Add("*", new StaticFactoryToken("*", TokenType.Multiply));
            staticTokens.Add("/", new StaticFactoryToken("/", TokenType.Devide));

            staticTokens.Add("=", new StaticFactoryToken("=", TokenType.EqualOrSet));
            staticTokens.Add("!=", new StaticFactoryToken("!=", TokenType.Unequal));
            staticTokens.Add(">", new StaticFactoryToken(">", TokenType.Greater));
            staticTokens.Add("<", new StaticFactoryToken("<", TokenType.Smaller));
            staticTokens.Add(">=", new StaticFactoryToken(">=", TokenType.GreaterEqual));
            staticTokens.Add("<=", new StaticFactoryToken("<=", TokenType.SmallerEqual));

            staticTokens.Add("and", new StaticFactoryToken("and", TokenType.And));
            staticTokens.Add("or", new StaticFactoryToken("or", TokenType.Or));

            staticTokens.Add("(", new StaticFactoryToken("(", TokenType.OpenParenthesis));
            staticTokens.Add(")", new StaticFactoryToken(")", TokenType.CloseParenthesis));
            staticTokens.Add("[", new StaticFactoryToken("[", TokenType.OpenBracket));
            staticTokens.Add("]", new StaticFactoryToken("]", TokenType.CloseBracket));
            staticTokens.Add("{", new StaticFactoryToken("{", TokenType.OpenBraceToken));
            staticTokens.Add("}", new StaticFactoryToken("}", TokenType.CloseBraceToken));

            // Create list of dynamic factory tokens
            dynamicTokens = new Dictionary<TokenType, DynamicFactoryToken>();

            dynamicTokens.Add(TokenType.Constant, new DynamicFactoryToken(TokenType.Constant));
            dynamicTokens.Add(TokenType.Name, new DynamicFactoryToken(TokenType.Name));

            staticTokens.Add("select", new StaticFactoryToken("select", TokenType.Select));
            staticTokens.Add("from", new StaticFactoryToken("from", TokenType.From));
            staticTokens.Add("where", new StaticFactoryToken("where", TokenType.Where));
            staticTokens.Add("insert", new StaticFactoryToken("insert", TokenType.Insert));
            staticTokens.Add("into", new StaticFactoryToken("into", TokenType.Into));
            staticTokens.Add("values", new StaticFactoryToken("values", TokenType.Values));
            staticTokens.Add("?", new StaticFactoryToken("?", TokenType.Parameter));
            staticTokens.Add("distinct", new StaticFactoryToken("distinct", TokenType.Distinct));
            staticTokens.Add("in", new StaticFactoryToken("in", TokenType.In));
            staticTokens.Add("as", new StaticFactoryToken("as", TokenType.As));
            staticTokens.Add("not", new StaticFactoryToken("not", TokenType.Not));
            staticTokens.Add("first", new StaticFactoryToken("first", TokenType.First));
        }
        #endregion

        #region Public Member
        /// <summary>
        /// List with all dynamic token factories
        /// </summary>
        internal IDictionary<TokenType, DynamicFactoryToken> DynamicTokens
        {
            get { return dynamicTokens; }
        }

        /// <summary>
        /// List with all static token factories
        /// </summary>
        internal IDictionary<string, StaticFactoryToken> StaticTokens
        {
            get { return staticTokens; }
        }
        #endregion
    }
}
