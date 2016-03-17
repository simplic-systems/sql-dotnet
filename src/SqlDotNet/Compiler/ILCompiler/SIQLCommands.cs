using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Contains all SIQL-Commands. If a command ends with _PREP, it is prepared for use with place-holders for using with string.format.
    /// </summary>
    public static class SIQLCommands
    {
        /// <summary>
        /// Returns @siql is is the beginning of every siql "program"
        /// </summary>
        public const string SIQL_TAG = "@siql";

        /// <summary>
        /// Load a constant value onto the stack
        /// </summary>
        public const string LOAD_CONST = "ldc";

        /// <summary>
        /// <see cref="LOAD_CONST"/> with .{0} {1}
        /// {0}: Data type
        /// {1}: Value
        /// </summary>
        public const string LOAD_CONST_PREP = LOAD_CONST + ".{0} {1}";

        /// <summary>
        /// Open a cursor for iterating over sources/values and generate results
        /// </summary>
        public const string CURSOR_OPEN = "ocur";

        /// <summary>
        /// <see cref="CURSOR_OPEN"/> .{0} {1}
        /// {0}: Cursor type, currently only table
        /// {1}: Name of the cursor. Should start with curx (x is the number)
        /// </summary>
        public const string CURSOR_OPEN_PREP = CURSOR_OPEN + ".{0} {1}";

        /// <summary>
        /// Defines the cursor output definition
        /// </summary>
        public const string CURSOR_OUTPUT_DEFINITION = "curout";

        /// <summary>
        /// <see cref="CURSOR_OUTPUT_DEFINITION"/> .{0} {1}
        /// {0}: Cursor name
        /// {1}: List of return-columns ffor the given source
        /// </summary>
        public const string CURSOR_OUTPUT_DEFINITION_PREP = CURSOR_OUTPUT_DEFINITION + ".{0} ({1})";

        /// <summary>
        /// Defines the source, which fills the query based on the query type from ocur.
        /// Is only required if open cursor type is not none (first parameter)
        /// </summary>
        public const string CURSOR_SOURCE = "cursrc";

        /// <summary>
        /// <see cref="CURSOR_SOURCE"/> .{0} {1}
        /// {0}: Cursoe name
        /// {1}: Table name
        /// </summary>
        public const string CURSOR_SOURCE_PREP = CURSOR_SOURCE + ".{0} {1}";

        /// <summary>
        /// Open a new result set, which will be filled e.g. by a cursor
        /// </summary>
        public const string RESULTSET_OPEN = "oresset";

        /// <summary>
        /// <see cref="RESULTSET_OPEN"/> {0}
        /// {0}: Name of the result-set. In general starts with resX (X is increasing number)
        /// </summary>
        public const string RESULTSET_OPEN_PREP = RESULTSET_OPEN + " {0}";

        /// <summary>
        /// Defines a cursor filter
        /// </summary>
        public const string CURSOR_FILTER = "filter";

        /// <summary>
        /// <see cref="CURSOR_FILTER"/> {0}
        /// {0}: Cursor which will be filtered
        /// </summary>
        public const string CURSOR_FILTER_PREP = CURSOR_FILTER + " {0}";

        /// <summary>
        /// Defines a term which will be used to fill a result set
        /// </summary>
        public const string RESULTSET_FILL = "fresset";

        /// <summary>
        /// <see cref="RESULTSET_FILL"/>.{0} (1)
        /// {0}: Name of the result set
        /// {1}: List of cursoers
        /// </summary>
        public const string RESULTSET_FILL_PREP = RESULTSET_FILL + ".{0} {1}";

        /// <summary>
        /// Create a new row in a result set
        /// </summary>
        public const string RESULTSET_CREATE_ROW = "crow";

        /// <summary>
        /// Pop the value on the stack to the next column in the result-set
        /// </summary>
        public const string RESULTSET_POP_TO_NEXT_COLUMN = "pnxc";

        #region [Operator]
        /// <summary>
        /// Add operator
        /// </summary>
        public const string OP_ADD = "add";

        /// <summary>
        /// Subtract operator
        /// </summary>
        public const string OP_SUB = "sub";

        /// <summary>
        /// Multiply operator
        /// </summary>
        public const string OP_MUL = "mul";

        /// <summary>
        /// Divide operator
        /// </summary>
        public const string OP_DIV = "div";

        /// <summary>
        /// And boolean operator
        /// </summary>
        public const string OP_AND = "and";

        /// <summary>
        /// Or boolean operator
        /// </summary>
        public const string OP_OR = "or";

        /// <summary>
        /// Equal boolean operator
        /// </summary>
        public const string OP_EQ = "eq";

        /// <summary>
        /// Unquel boolean operator
        /// </summary>
        public const string OP_UEQ = "ueq";

        /// <summary>
        /// Smaller boolean operator
        /// </summary>
        public const string OP_SM = "sm";

        /// <summary>
        /// Greater boolean operator
        /// </summary>
        public const string OP_GT = "gt";

        /// <summary>
        /// Smaller-Equal boolean operator
        /// </summary>
        public const string OP_SMEQ = "smeq";

        /// <summary>
        /// Greater-Equal boolean operator
        /// </summary>
        public const string OP_GTEQ = "gteq";
        #endregion
    }
}
