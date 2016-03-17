using SqlDotNet.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// Stack item instance
    /// </summary>
    internal struct StackItem
    {
        /// <summary>
        /// Value
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Type
        /// </summary>
        public DataType DataType
        {
            get;
            set;
        }
    }
}
