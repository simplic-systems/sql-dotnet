using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.CLRInterface
{
    /// <summary>
    /// Execute a query and triggers IQueryExecutor instances
    /// </summary>
    public class QueryExecutor
    {
        #region Public Methods
        /// <summary>
        /// Execute a query
        /// </summary>
        /// <param name="executor">Instance which should execute the query</param>
        /// <param name="query">Compiled query result</param>
        /// <param name="parameter">List of available/passed parameter which should be replaced for sql-?</param>
        /// <param name="errorListener">Error reporting instance</param>
        /// <returns>Result of a sql query or the amount of affected data</returns>
        public object Execute(IQueryExecutor executor, Compiler.CompiledQuery query, IList<QueryParameter> parameter, IErrorListener errorListener)
        {
            object returnValue = null;

            if (query.EntryPoint != null && query.EntryPoint.Children.Count > 0)
            {
                var root = query.EntryPoint.Children.First();

                // Execute select statement
                if (root.NodeType == Compiler.SyntaxNodeType.Select)
                {

                }
                // Execute insert statement
                else if (root.NodeType == Compiler.SyntaxNodeType.Insert)
                {
                    var into = root.FindChildrenOfType<Compiler.IntoNode>().FirstOrDefault();
                    string tableName = "";
                    IList<Compiler.ColumnNode> columns = new List<Compiler.ColumnNode>();
                    IList<QueryParameter> parameterToPass = new List<QueryParameter>();

                    // Table
                    if (into != null)
                    {
                        var table = into.FindChildrenOfType<Compiler.TableNode>().FirstOrDefault();

                        if (table != null)
                        {
                            // table name with owner. e.g.: <owner>.<user>.<table>...
                            tableName = (table.Owner == null ? "" : table.Owner + ".") + table.TableName;
                        }
                        else
                        {
                            throw new Exception("Missing table after into statement.");
                        }
                        
                        // Columns
                        columns = into.FindChildrenOfType<Compiler.ColumnNode>();
                    }
                    else
                    {
                        throw new Exception("Missing into statement behind insert.");
                    }
                    
                    // Use values tatements
                    var values = root.FindChildrenOfType<Compiler.ValuesNode>().FirstOrDefault();
                    if (values != null)
                    {
                        int parameterCount = 0;

                        foreach (var value in values.Children)
                        {
                            switch (value.NodeType)
                            {
                                case Compiler.SyntaxNodeType.Constant:
                                    object val = Compiler.DataTypeHelper.StringValueToObject(value.Token.Content, ((Compiler.ConstantNode)value).DataType);

                                    if (((Compiler.ConstantNode)value).DataType == Compiler.DataType.Str)
                                    {
                                        val = val.ToString().Substring(1, val.ToString().Length - 2);
                                    }

                                    parameterToPass.Add(new QueryParameter() { Name = value.DebugText, Value = val });
                                    break;

                                case Compiler.SyntaxNodeType.Parameter:
                                    var p = parameter.ElementAtOrDefault(parameterCount);

                                    if (p != null)
                                    {
                                        parameterToPass.Add(parameter[parameterCount]);
                                    }
                                    else
                                    {
                                        throw new Exception("Paramter amount does not match host variable amount. (?)");
                                    }

                                    parameterCount++;
                                    break;
                            }
                        }
                    }

                    // Execute insert
                    returnValue = executor.Insert(tableName, columns, parameterToPass);
                }
                else
                {
                    throw new Exception("Query is not executable, wrong entry point (use select, or insert)");
                }
            }

            return returnValue;
        }
        #endregion
    }
}
