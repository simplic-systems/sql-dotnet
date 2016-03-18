using SqlDotNet.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDotNet.CLRInterface;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// Cursor scope item
    /// </summary>
    public class Cursor
    {
        #region Private Member
        private string name;
        private IList<CLRInterface.QueryResultRow> rows;
        #endregion

        #region Constructor
        /// <summary>
        /// Create cursor
        /// </summary>
        /// <param name="name">Unique cursor name</param>
        public Cursor(string name)
        {
            this.name = name;
        }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        #endregion

        #region Public Member
        /// <summary>
        /// Get variable name
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        public IList<QueryResultRow> Rows
        {
            get
            {
                return rows;
            }
            internal set
            {
                rows = value;
            }
        }
        #endregion
    }
}
