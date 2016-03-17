using SqlDotNet.CLRInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDotNet.Compiler;

namespace SqlDotNet.Debugger
{
    internal class Executor : IQueryExecutor
    {
        public object CallFunction(string name, IList<QueryParameter> parameter)
        {
            return 1234;
        }

        public int Insert(string tableName, IList<ColumnNode> columns, IList<QueryParameter> parameter)
        {
            return 123;
        }
    }
}
