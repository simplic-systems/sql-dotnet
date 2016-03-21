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
        private void AssertFunction(string name, IList<QueryParameter> parameter, int minParameter, int maxParameter, DataType[] types)
        {
            if (parameter.Count > maxParameter || parameter.Count < minParameter)
            {
                throw new ArgumentException(string.Format("`{0}` expected at least {1} parameter.", name, minParameter));
            }
        }

        public Tuple<object, DataType> CallFunction(string name, IList<QueryParameter> parameter)
        {
            name = name.ToLower();
            if (!string.IsNullOrWhiteSpace(name))
            {
                switch (name)
                {
                    #region [Concat]
                    case "concat":
                        {
                            if (parameter == null || parameter.Count < 1)
                            {
                                throw new ArgumentException("`Concat` requires atleast one argument.");
                            }

                            StringBuilder sb = new StringBuilder();
                            foreach (var p in parameter)
                            {
                                sb.Append(p.Value);
                            }
                            return new Tuple<object, DataType>(sb.ToString(), DataType.Str);
                        }
                    #endregion

                    #region [Trim]
                    case "ltrim":
                        {
                            AssertFunction(name, parameter, 1, 1, new DataType[] { DataType.Str });

                            return new Tuple<object, DataType>(parameter.First().Value.ToString().TrimStart(), DataType.Str);
                        }
                    case "rtrim":
                        {
                            AssertFunction(name, parameter, 1, 1, new DataType[] { DataType.Str });

                            return new Tuple<object, DataType>(parameter.First().Value.ToString().TrimEnd(), DataType.Str);
                        }
                    case "trim":
                        {
                            AssertFunction(name, parameter, 1, 1, new DataType[] { DataType.Str });

                            return new Tuple<object, DataType>(parameter.First().Value.ToString().Trim(), DataType.Str);
                        }
                    #endregion

                    #region [Len]
                    case "len":
                        {
                            AssertFunction(name, parameter, 1, 1, new DataType[] { DataType.Str });

                            return new Tuple<object, DataType>(parameter.First().Value.ToString().Length, DataType.Int32);
                        }
                    #endregion

                    #region [Lower/Upper]
                    case "lower":
                        {
                            AssertFunction(name, parameter, 1, 1, new DataType[] { DataType.Str });

                            return new Tuple<object, DataType>(parameter.First().Value.ToString().ToLower(), DataType.Int32);
                        }

                    case "upper":
                        {
                            AssertFunction(name, parameter, 1, 1, new DataType[] { DataType.Str });

                            return new Tuple<object, DataType>(parameter.First().Value.ToString().ToUpper(), DataType.Int32);
                        }
                    #endregion

                    case "current_date":
                    case "curdate":
                    case "current_time":
                    case "curtime":
                        {
                            if (parameter == null || parameter.Count == 0)
                            {
                                return new Tuple<object, DataType>(DateTime.Now, DataType.Object);
                            }
                            else
                            {
                                throw new ArgumentException("`current_date` does not expect any arguments");
                            }
                        }


                }
            }

            throw new Exception("Could not found function " + name ?? "--noname--");
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

            row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });
            returnValue.Add(row);

            row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });
            returnValue.Add(row);

            row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });
            returnValue.Add(row);

            row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });
            returnValue.Add(row);

            row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });
            returnValue.Add(row);

            row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });
            returnValue.Add(row);

            row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });
            returnValue.Add(row);

            row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });
            returnValue.Add(row);

            row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });
            returnValue.Add(row);

            row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });
            returnValue.Add(row);

            row = new QueryResultRow();
            row.Columns.Add("archiv_guid", Guid.NewGuid());
            row.Columns.Add("archiv_blob", new byte[] { 1, 23, 213, 24 });
            returnValue.Add(row);

            return returnValue;
        }
    }
}
