using SqlDotNet.CLRInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDotNet.Compiler;
using SqlDotNet.Schema;

namespace SqlDotNet.Debugger
{
    internal class Executor : IQueryExecutor
    {
        public object CallFunction(string name, IList<QueryParameter> parameter)
        {
            return 1234;
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

        public int Insert(string tableName, IList<ColumnNode> columns, IList<QueryParameter> parameter)
        {
            return 123;
        }
    }
}
