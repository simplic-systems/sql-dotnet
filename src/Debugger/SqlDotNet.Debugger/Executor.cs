using SqlDotNet.CLRInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDotNet.Compiler;
using SqlDotNet.Schema;
using SqlDotNet.Runtime;

namespace SqlDotNet.Debugger
{
    internal class Executor : IQueryExecutor
    {
        public Tuple<object, DataType> CallFunction(string name, IList<QueryParameter> parameter)
        {
            return new Tuple<object, DataType>(12, DataType.Int32);
        }

        public TableDefinition GetTableSchema(string owner, string table)
        {
            if (table.ToLower() == "archiv")
            {
                var tbl = new TableDefinition();
                tbl.Columns.Add(new ColumnDefinition() { Name = "archiv_guid", NotNull = true });
                tbl.Columns.Add(new ColumnDefinition() { Name = "archiv_blob", NotNull = true });

                return tbl;
            }

            throw new Exception("Table " + table ?? "" + " not found");
        }

        public int Insert(string tableName, IList<string> columns, IList<QueryParameter> parameter)
        {
            return 123;
        }

        public IList<QueryResultRow> Select(string tableName, bool isScalar, bool distinctValues, IList<ColumnDefinition> columns, FilterCursorCCNode filter, Scope parameter)
        {
            var returnValue = new List<QueryResultRow>();

            var row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });

            returnValue.Add(row);

            return returnValue;
        }
    }
}
