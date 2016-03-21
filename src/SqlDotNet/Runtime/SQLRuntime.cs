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
            if (node is OpenCursor)
            {
                var openCursorNode = (OpenCursor)node;
                var cursor = scope.CreateCursor(openCursorNode.CursorName);

                FilterCursor filter = openCursorNode.FindChildrenOfType<FilterCursor>().FirstOrDefault();

                if (openCursorNode.CursorType == "tbl")
                {
                    cursor.Rows = executor.Select(openCursorNode.CursorSource, false, false, openCursorNode.Columns, filter, scope);
                }
            }
            #endregion

            else if (node is OpenResultSet)
            {
                var openResultSet = (OpenResultSet)node;
                var resultSet = scope.CreateResultSet(openResultSet.ResultSetName);

                var fill = openResultSet.FindChildrenOfType<FillResultSet>().FirstOrDefault();
                var cursor = scope.GetCursor(fill.Cursor);

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

            else if (node is Runtime.CallFunctionNode)
            {
                var callFunc = (Runtime.CallFunctionNode)node;

                // Execute insert
                if (callFunc.Type == "_insert_into")
                {
                    // Get stack as array
                    IList<QueryParameter> arguments = new List<QueryParameter>();
                    while (scope.ArgumentStack.Count > 0)
                    {
                        QueryParameter parameter = new QueryParameter();
                        parameter.Value = scope.ArgumentStack.PopFirst().Item2.Value;

                        arguments.Add(parameter);
                    }

                    executor.Insert(callFunc.FunctionName, callFunc.Arugments, arguments);
                }
            }

            #region [LoadConstantNode]
            // Constant handling
            // Push constant node to the stack
            else if (node is LoadConstantNode)
            {
                var constNode = (node as LoadConstantNode);
                scope.Stack.Push(constNode.ConstantValue, constNode.DataType);
            }
            #endregion

            // Constant handling
            // Push constant node to the stack
            else if (node is LoadColumnNode)
            {
                var colNode = (node as LoadColumnNode);

                scope.Stack.Push("Test", Compiler.DataType.Object);
            }

            else if (node is LoadArgumentNode)
            {
                var argNode = (node as LoadArgumentNode);

                var topItem = scope.Stack.Pop();
                scope.ArgumentStack.PushBack(new Tuple<int, StackItem>(argNode.Id, topItem));
            }

            #region [OperatorNode]
            // Operator
            else if (node is OperatorNode)
            {
                scope.Stack.Execute((node as OperatorNode).OpType);
            }
            #endregion

            #region [CreateResultSetRow]
            else if (node is CreateResultSetRow)
            {
                var rs = scope.GetResultSet();
                rs.Rows.Add(new QueryResultRow());
            }
            #endregion

            #region [PopToNextColumn]
            // Operator
            else if (node is PopToNextColumn)
            {
                var ptnxcNode = (PopToNextColumn)node;
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

        #endregion
    }
}
