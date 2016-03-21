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
    /// ResultSet scope item
    /// </summary>
    public class ResultSet
    {
        #region Private Member
        private string name;
        private IList<CLRInterface.QueryResultRow> rows;
        private IDictionary<string, DataType> definition;
        #endregion

        #region Constructor
        /// <summary>
        /// Create ResultSet
        /// </summary>
        /// <param name="name">Unique result set name</param>
        public ResultSet(string name)
        {
            this.name = name;
            rows = new List<QueryResultRow>();
            Cursors = new List<string>();
            definition = new Dictionary<string, DataType>(StringComparer.OrdinalIgnoreCase);
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

        /// <summary>
        /// List of cursors which fills this result set, currently only one supported
        /// </summary>
        public IList<string> Cursors
        {
            get;
            private set;
        }

        /// <summary>
        /// Result-Set definition
        /// </summary>
        public IDictionary<string, DataType> Definition
        {
            get
            {
                return definition;
            }

            set
            {
                definition = value;
            }
        }
        #endregion
    }
}
