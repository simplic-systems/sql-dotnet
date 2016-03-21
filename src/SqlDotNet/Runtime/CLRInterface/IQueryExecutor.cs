using SqlDotNet.Compiler;
using SqlDotNet.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.CLRInterface
{
    /// <summary>
    /// Interface which must be implemented to execute parsed sql queries. If this interface is implemented correctly,
    /// the library is designed to work in any way.
    /// </summary>
    public interface IQueryExecutor
    {
        /// <summary>
        /// Insert data into some table
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="columns">List of columns. If no columns are called, used an ordered object id or match by types</param>
        /// <param name="parameter">List of parameter</param>
        /// <returns>Amount of affected rows</returns>
        int Insert(string tableName, IList<string> columns, IList<QueryParameter> parameter);

        IList<QueryResultRow> Select(string tableName, bool isScalar, bool distinctValues, IList<ColumnDefinition> columns, Runtime.FilterCursorCCNode filter, Runtime.Scope parameter);

        /// <summary>
        /// Call an sql function or aggregation function
        /// </summary>
        /// <param name="name">Name of the function</param>
        /// <param name="parameter">List of parameter</param>
        /// <returns>Return value as object with specific type</returns>
        Tuple<object, DataType> CallFunction(string name, IList<QueryParameter> parameter);

        /// <summary>
        /// Get the table definitions
        /// </summary>
        /// <param name="owner">Table owner (user, db, ...)</param>
        /// <param name="table">Table name</param>
        /// <returns></returns>
        TableDefinition GetTableSchema(string owner, string table);
    }
}
