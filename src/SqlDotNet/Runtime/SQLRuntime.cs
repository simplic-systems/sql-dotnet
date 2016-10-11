using Simplic.Collections.Generic;
using SqlDotNet.CLRInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Runtime
{
    /// <summary>
    /// SQL-Runtime core
    /// </summary>
    public class SCLRuntime
    {
        #region Private Member
        private Scope rootScope;
        private CLRInterface.IQueryExecutor executor;
        private int runtimeCursor;
        #endregion

        #region Constructor
        /// <summary>
        /// Create SCL-Runtime
        /// </summary>
        /// <param name="executor">Pass clr interface</param>
        /// <param name="parameter">List of available parameter</param>
        public SCLRuntime(CLRInterface.IQueryExecutor executor, IList<QueryParameter> parameter)
        {
            // create basic scope
            rootScope = new Scope(null);
            this.executor = executor;

            int unnamedCounter = 0;
            foreach (var _var in parameter)
            {
                string _varName = _var.Name;

                if (string.IsNullOrWhiteSpace(_varName))
                {
                    _varName = string.Format("__unnamed{0}", unnamedCounter);
                    unnamedCounter++;
                }

                var newVar = rootScope.CreateVariable(_varName);
                newVar.Value = _var.Value;
            }
        }
        #endregion

        #region Private Methods

        #region [Execute Command]
        /// <summary>
        /// Execute a single command, cann be classed recursive
        /// </summary>
        /// <param name="node">ChainNode instance (command)</param>
        /// <param name="scope">Current scope, in which the command is executed</param>
        private void ExecuteCommand(CommandChainNode node, Scope scope)
        {
            #region [OpenCursor]
            if (node is OpenCursorCCNode)
            {
                var openCursorNode = (OpenCursorCCNode)node;
                var cursor = scope.CreateCursor(openCursorNode.CursorName);

                FilterCursorCCNode filter = openCursorNode.FindChildrenOfType<FilterCursorCCNode>().FirstOrDefault();

                if (openCursorNode.CursorType == "tbl")
                {
                    cursor.Rows = executor.Select(openCursorNode.CursorSource, false, false, openCursorNode.Columns, filter, scope);
                }
            }
            #endregion

            else if (node is OpenResultSetCCNode)
            {
                var openResultSet = (OpenResultSetCCNode)node;
                var resultSet = scope.CreateResultSet(openResultSet.ResultSetName);

                foreach (var def in openResultSet.ResultSetDefinition)
                {
                    resultSet.Definition.Add(def, Compiler.DataType.None);
                }

                var fill = openResultSet.FindChildrenOfType<FillResultSetCCNode>().FirstOrDefault();


                Cursor cursor = null;

                // This will be used for select * from ....
                if (fill.Cursor != null)
                {
                    cursor = scope.GetCursor(fill.Cursor);
                    resultSet.Cursors.Add(cursor.Name);
                }
                // This will be used for select func(1, 2, ...) ... !Without from clause
                else
                {
                    cursor = scope.CreateCursor("runtimeCursor" + runtimeCursor.ToString());
                    cursor.Rows = new List<QueryResultRow>();
                    cursor.Rows.Add(new QueryResultRow()); // Add emtpy dummy row
                    cursor.CurrentRow = 0;
                    runtimeCursor++;
                }

                for (int i = 0; i < cursor.Rows.Count; i++)
                {
                    cursor.CurrentRow = i;
                    var cursorScope = scope.GetNew();

                    foreach (var fillNode in fill.Children)
                    {
                        ExecuteCommand(fillNode, cursorScope);
                    }
                }
            }

            #region [CallFunctionNode]
            else if (node is Runtime.CallFunctionCCNode)
            {
                var callFunc = (Runtime.CallFunctionCCNode)node;

                // Execute insert
                if (callFunc.Type == "_insert_into")
                {
                    // Get stack as array
                    Dequeue<QueryParameter> arguments = new Dequeue<QueryParameter>();
                    while (scope.ArgumentStack.Count > 0)
                    {
                        QueryParameter parameter = new QueryParameter();
                        parameter.Value = scope.ArgumentStack.PopFirst().Item2.Value;

                        // We need to push to the front here, because we have to invert the stack.
                        // For example, calling fun (1, 2, 3) puts 3, 2, 1 on the stack. By adding tham backwards, the list will be correct later
                        arguments.PushFront(parameter);
                    }

                    var amount = executor.Insert(callFunc.FunctionName, callFunc.Arugments, arguments.ToList());
                    scope.Stack.Push(amount, Compiler.DataType.Int64);

                    // Create resultset for insert
                    var rs = scope.GetResultSet();
                    rs.Definition.Add("__amount__", Compiler.DataType.Int32);
                    var row = new QueryResultRow();
                    row.Columns.Add("__amount__", amount);
                }
                if (callFunc.Type == "f")
                {
                    // Get stack as array
                    Dequeue<QueryParameter> arguments = new Dequeue<QueryParameter>();
                    for(int i = 0; i < callFunc.Arugments.Count; i++)
                    {
                        QueryParameter parameter = new QueryParameter();
                        parameter.Value = scope.ArgumentStack.PopFirst().Item2.Value;

                        // We need to push to the front here, because we have to invert the stack.
                        // For example, calling fun (1, 2, 3) puts 3, 2, 1 on the stack. By adding tham backwards, the list will be correct later
                        arguments.PushFront(parameter);
                    }

                    var res = executor.CallFunction(callFunc.FunctionName, arguments.ToList());
                    scope.Stack.Push(res.Item1, res.Item2);
                }
            }
            #endregion

            #region [LoadConstantNode]
            // Constant handling
            // Push constant node to the stack
            else if (node is LoadConstantCCNode)
            {
                var constNode = (node as LoadConstantCCNode);
                scope.Stack.Push(constNode.ConstantValue, constNode.DataType);
            }
            #endregion

            #region [LoadParameterNode]
            // Constant handling
            // Push constant node to the stack
            else if (node is LoadParameterCCNode)
            {
                var parameterNode = (node as LoadParameterCCNode);

                var index = parameterNode.Index;

                if (rootScope.Vars.Count < index)
                {
                    throw new Exception("Not enough parameter for variables");
                }

                var _var = rootScope.Vars.ElementAt(index);

                scope.Stack.Push(_var.Value.Value, _var.Value.DataType);
            }
            #endregion

            // Constant handling
            // Push constant node to the stack
            else if (node is LoadColumnCCNode)
            {
                var colNode = (node as LoadColumnCCNode);

                var resultSet = scope.GetResultSet();
                var cursor = scope.GetCursor(resultSet.Cursors.First());

                var row = cursor.Rows[cursor.CurrentRow];
                var col = row.Columns[colNode.Name];

                scope.Stack.Push(col, Compiler.DataType.Object);
            }

            #region [LoadArgumentNode]
            else if (node is LoadArgumentCCNode)
            {
                var argNode = (node as LoadArgumentCCNode);

                var topItem = scope.Stack.Pop();
                scope.ArgumentStack.PushFront(new Tuple<int, StackItem>(argNode.Id, topItem));
            }
            #endregion

            #region [OperatorNode]
            // Operator
            else if (node is OperatorCCNode)
            {
                scope.Stack.Execute((node as OperatorCCNode).OpType);
            }
            #endregion

            #region [CreateResultSetRow]
            else if (node is CreateResultSetRowCCNode)
            {
                var rs = scope.GetResultSet();
                rs.Rows.Add(new QueryResultRow());
            }
            #endregion

            #region [PopToNextColumn]
            // Operator
            else if (node is PopToNextColumnCCNode)
            {
                var ptnxcNode = (PopToNextColumnCCNode)node;
                var _top = scope.Stack.Pop();
                object val = _top.Value;

                var rs = scope.GetResultSet();

                if (rs.Rows == null || rs.Rows.Count == 0)
                {
                    throw new Exception("Could not push to result set, because no row was created first.");
                }

                rs.Rows.Last().Columns.Add(ptnxcNode.ColumnName, val);
            }
            #endregion
        }
        #endregion

        #endregion

        #region Public Methods

        #region [Execute interface]
        /// <summary>
        /// Execute siql code
        /// </summary>
        internal void Execute(Stream ilCode)
        {
            throw new NotImplementedException("Not yet implemented. SIQL parser is not finished.");
        }

        /// <summary>
        /// Execute command chain node directly
        /// </summary>
        /// <param name="root">Root command chain item</param>
        internal void Execute(CommandChainNode root)
        {
            foreach (var node in root.Children)
            {
                ExecuteCommand(node, this.rootScope);
            }
        }
        #endregion

        #endregion

        #region Public Member
        /// <summary>
        /// Get the result of the sql-statement
        /// </summary>
        public ResultSet ResultSet
        {
            get
            {
                return rootScope.GetResultSet();
            }
        }
        #endregion
    }
}
