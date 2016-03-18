using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Schema
{
    /// <summary>
    /// Contains the definition of an sql table
    /// </summary>
    public class TableDefinition
    {
        /// <summary>
        /// Create table definition
        /// </summary>
        public TableDefinition()
        {
            Columns = new List<ColumnDefinition>();
        }

        /// <summary>
        /// Column list
        /// </summary>
        public IList<ColumnDefinition> Columns
        {
            get;
            private set;
        }
    }
}
