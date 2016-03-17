using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Contains all information about a token. The main properties are the token content, and the token place (line and index)
    /// </summary>
    public class RawToken
    {
        #region Constructor
        /// <summary>
        /// Create new token
        /// </summary>
        /// <param name="content">Token content</param>
        /// <param name="line">Start and end position of the token (line)</param>
        /// <param name="index">Start and end position of the token (index/chars)</param>
        public RawToken(string content, Tuple<int, int> line, Tuple<int, int> index)
        {
            Content = content;
            Line = line;
            Index = index;
        }
        #endregion

        #region Public Member
        /// <summary>
        /// Start and end line of the token
        /// </summary>
        public Tuple<int, int> Line
        {
            get;
            private set;
        }

        /// <summary>
        /// Start and end index of the token.
        /// </summary>
        public Tuple<int, int> Index
        {
            get;
            private set;
        }

        /// <summary>
        /// Token content
        /// </summary>
        public string Content
        {
            get;
            internal set;
        }

        /// <summary>
        /// Defines the type of the token
        /// </summary>
        public TokenType Type
        {
            get;
            set;
        }
        #endregion
    }
}
