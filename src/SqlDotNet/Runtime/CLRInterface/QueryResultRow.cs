using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.CLRInterface
{
    /// <summary>
    /// Contains the result of a select statement as row.
    /// </summary>
    public class QueryResultRow
    {
        #region Private
        private IDictionary<string, object> columns;
        #endregion

        #region Constructor
        /// <summary>
        /// Create result row
        /// </summary>
        public QueryResultRow()
        {
            columns = new Dictionary<string, object>();
        }
        #endregion

        #region Public Member
        /// <summary>
        /// List of all columns
        /// </summary>
        public IDictionary<string, object> Columns
        {
            get
            {
                return columns;
            }
        }
        #endregion
    }
}
