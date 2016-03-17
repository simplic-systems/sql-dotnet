using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Executor
{
    /// <summary>
    /// Base query paraneter
    /// </summary>
    public class QueryParameter
    {
        /// <summary>
        /// Name of the parameter (maybe only `?` here)
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Value of the parameter. No DBNull, just use null.
        /// </summary>
        public object Value
        {
            get;
            set;
        }
    }
}
