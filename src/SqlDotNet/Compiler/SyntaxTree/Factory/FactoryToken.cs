using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Abstract token class, all tokens must inherit from this class
    /// </summary>
    internal abstract class FactoryToken
    {
        #region Private Member
        private TokenType type;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the token
        /// </summary>
        public FactoryToken(TokenType type)
        {
            this.type = type;
        }
        #endregion

        #region Public Methods
        public abstract SyntaxTreeNode GetSyntaxNode(RawToken token);
        #endregion

        #region Public Member
        public TokenType Type
        {
            get { return type; }
        }
        #endregion
    }
}
