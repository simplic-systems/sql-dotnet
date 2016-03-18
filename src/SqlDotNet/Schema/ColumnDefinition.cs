using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Schema
{
    /// <summary>
    /// Defines column properties
    /// </summary>
    public class ColumnDefinition
    {
        /// <summary>
        /// Name of the column
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// If true, null values are not allowed, else false (default)
        /// </summary>
        public bool NotNull
        {
            get;
            set;
        }
    }
}
