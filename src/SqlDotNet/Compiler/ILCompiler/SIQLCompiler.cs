using Simplic.Collections.Generic;
using SqlDotNet.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    /// <summary>
    /// Compiles a syntax tree to IL-Code (optimized for sql)
    /// </summary>
    public class SIQLCompiler : ICompiler
    {
        public const string SIQL_VERSION = "v1";

        #region Private Member
        private IErrorListener errorListener;
        private int cursorCounter;
        private int resultSetCounter;
        private CompiledQuery result;
        private CLRInterface.IQueryExecutor executor;
        #endregion

        #region Constructor
        /// <summary>
        /// Create compiler
        /// </summary>
        /// <param name="errorListener">Error listener instance</param>
        public SIQLCompiler(IErrorListener errorListener, CLRInterface.IQueryExecutor executor)
        {
            this.errorListener = errorListener;
            this.executor = executor;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Compile to IL-Code
        /// </summary>
        /// <param name="node">TreeNode instnace</param>
        /// <returns>Stream with the compiled code</returns>
        public CompiledQuery Compile(EntryPointNode node)
        {
            result = new CompiledQuery();
            result.CommandChainRoot = new RootCCNode(null);
            ((RootCCNode)(result.CommandChainRoot)).Version = SIQL_VERSION;

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine("// SQL program");
            strBuilder.AppendLine();
            strBuilder.AppendLine(string.Format("{0} {1}", SIQLCommands.SIQL_TAG, SIQL_VERSION));
            strBuilder.AppendLine();

            // Compile to output
            foreach (var child in node.Children)
            {
                Compile(strBuilder, child, result.CommandChainRoot, 0);
            }

            // Return code
            result.ILCode = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(strBuilder.ToString()));
            result.EntryPoint = node as EntryPointNode;
            return result;
        }
        #endregion

        #region Private Member
        private void Compile(StringBuilder strBuilder, SyntaxTreeNode node, CommandChainNode parent, int intendend)
        {
            string intendendStr = new string('\t', intendend);

            switch (node.NodeType)
            {
                #region [SyntaxNodeType.Select]
                case SyntaxNodeType.Select:
                    {
                        var table = node.FindFirstOrDefaultByPath<FromNode, TableNode>();

                        string cursorName = null;

                        // Use for select from a table
                        if (table != null)
                        {
                            var tblDef = executor.GetTableSchema(table.Owner, table.TableName);

                            /*TODO:
                            Some where here we have to check for columns which does can not be found in any table definition,
                            are available in multiple table definition or other stuff that is not allowed to happen...
                            */

                            // Open new cursor
                            cursorCounter++;
                            int cursorNr = cursorCounter;
                            cursorName = GetCursorName(cursorNr);

                            // Output definition
                            StringBuilder columnsStr = new StringBuilder();
                            foreach (var col in tblDef.Columns)
                            {
                                if (columnsStr.Length > 0)
                                {
                                    columnsStr.Append(", ");
                                }
                                columnsStr.Append(col.Name);
                            }

                            strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.CURSOR_OPEN_PREP, "tbl", cursorName, columnsStr.ToString()));
                            var openCursorNode = parent.CreateNode<OpenCursorCCNode>();
                            openCursorNode.Columns = tblDef.Columns.ToList();
                            openCursorNode.CursorName = cursorName;
                            openCursorNode.CursorType = "tbl";

                            // Cursor source
                            string tableName = table.TableName;
                            if (!string.IsNullOrWhiteSpace(table.Owner))
                            {
                                tableName = table.Owner + "." + table.TableName;
                            }

                            strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.CURSOR_SOURCE_PREP, cursorName, tableName));
                            openCursorNode.CursorSource = tableName;

                            // Append filter
                            var whereNode = node.FindFirstOrDefaultChildrenOfType<WhereNode>();
                            var cursorFilter = openCursorNode.CreateNode<FilterCursorCCNode>();

                            if (whereNode != null)
                            {
                                strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.CURSOR_FILTER_PREP, cursorName));
                                strBuilder.AppendLine(intendendStr + "{");

                                CompileExpression(strBuilder, node.FindFirstOrDefaultChildrenOfType<WhereNode>(), cursorFilter, intendend + 1);

                                strBuilder.AppendLine(intendendStr + "}");
                            }
                        }

                        // Create result-set stuff
                        resultSetCounter++;
                        int resultSetNr = resultSetCounter;
                        string resultSetName = GetResultSetName(resultSetNr);

                        ReturnValueList lst = node.FindFirstOrDefaultChildrenOfType<ReturnValueList>();

                        // Fill result-set column
                        int unnamedColumns = 0;
                        StringBuilder resultSetDefinition = new StringBuilder(); // Defines the returning columns
                        var resultSetDefnList = new List<string>();

                        foreach (var _node in lst.Children)
                        {
                            var alias = _node.FindFirstOrDefaultChildrenOfType<AsNode>();

                            if (resultSetDefinition.Length > 0)
                            {
                                resultSetDefinition.Append(", ");
                            }

                            if (alias == null || string.IsNullOrWhiteSpace(alias.Alias))
                            {
                                if (_node.Children.Count == 0 && _node is ColumnNode)
                                {
                                    var colChild = (ColumnNode)_node;
                                    resultSetDefnList.Add(colChild.ColumnName);
                                    resultSetDefinition.Append(colChild.ColumnName);

                                    continue;
                                }


                                resultSetDefnList.Add(string.Format("__col{0}", unnamedColumns));
                                resultSetDefinition.Append(string.Format("__col{0}", unnamedColumns));
                                unnamedColumns++;
                            }
                            else
                            {
                                resultSetDefinition.Append(alias.Alias);
                                resultSetDefnList.Add(alias.Alias);
                            }
                        }

                        strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.RESULTSET_OPEN_PREP, resultSetName, "(" + resultSetDefinition.ToString() + ")"));
                        var resultSetNode = parent.CreateNode<OpenResultSetCCNode>();
                        resultSetNode.ResultSetName = resultSetName;
                        resultSetNode.ResultSetDefinition = resultSetDefnList;

                        strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.RESULTSET_FILL_PREP, resultSetName, string.Format("({0})", (cursorName ?? ""))));
                        var fillResultSetNode = resultSetNode.CreateNode<FillResultSetCCNode>();
                        fillResultSetNode.Cursor = cursorName;

                        strBuilder.AppendLine(intendendStr + "{");

                        unnamedColumns = 0;
                        if (lst != null)
                        {
                            strBuilder.AppendLine("\t" + intendendStr + SIQLCommands.RESULTSET_CREATE_ROW);
                            fillResultSetNode.CreateNode<CreateResultSetRowCCNode>();

                            foreach (var _node in lst.Children)
                            {
                                // Some dummy node, only needed because CompileExpression iterates over children
                                // and needs a kind of "host"
                                var __d__ = new DummyNode(_node);

                                CompileExpression(strBuilder, __d__, fillResultSetNode, intendend + 1);

                                var alias = _node.FindFirstOrDefaultChildrenOfType<AsNode>();
                                string aliasStr = "";
                                if (alias != null && !string.IsNullOrWhiteSpace(alias.Alias))
                                {
                                    aliasStr = alias.Alias;
                                }
                                else
                                {
                                    if (_node.Children.Count == 0 && _node is ColumnNode)
                                    {
                                        var colChild = (ColumnNode)_node;
                                        aliasStr = colChild.ColumnName;
                                    }
                                    else
                                    {

                                        aliasStr = string.Format("__col{0}", unnamedColumns);
                                        unnamedColumns++;
                                    }
                                }

                                strBuilder.AppendLine("\t" + intendendStr + string.Format(SIQLCommands.RESULTSET_POP_TO_NEXT_COLUMN_REP, aliasStr));
                                fillResultSetNode.CreateNode<PopToNextColumnCCNode>().ColumnName = aliasStr;
                            }
                        }

                        strBuilder.AppendLine(intendendStr + "}");
                    }
                    break;
                #endregion

                #region [SyntaxNodeType.Column]
                case SyntaxNodeType.Column:
                    {
                        var columnNode = (node as ColumnNode);

                        string name = columnNode.ColumnName;

                        if (!string.IsNullOrWhiteSpace(columnNode.Owner))
                        {
                            name = columnNode.Owner + "." + name;
                        }

                        strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.LOAD_COLUMN_PREP, name));
                        var colNode = parent.CreateNode<LoadColumnCCNode>();
                        colNode.Name = columnNode.ColumnName;
                        colNode.Cursor = columnNode.Owner;
                    }
                    break;
                #endregion

                #region [SyntaxNodeType.Insert]
                case SyntaxNodeType.Insert:
                    {
                        var insertNode = (node as InsertNode);

                        //var callFunctionNode = parent.CreateNode<CallFunctionNode>();
                        var values = insertNode.FindFirstOrDefaultChildrenOfType<ValuesNode>();
                        if (values != null)
                        {
                            {
                                int i = 0;
                                foreach (var args in values.FindChildrenOfType<ArgumentNode>())
                                {
                                    CompileExpression(strBuilder, args, parent, intendend);

                                    // Crate CChainNode
                                    parent.CreateNode<LoadArgumentCCNode>().Id = i;
                                    strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.LOAD_ARGUMENT_PREP, i));
                                    i++;
                                }
                            }
                        }

                        // create call function node
                        var callFunction = parent.CreateNode<Runtime.CallFunctionCCNode>();

                        var into = insertNode.FindFirstOrDefaultChildrenOfType<IntoNode>();
                        if (into != null)
                        {
                            var table = into.FindFirstOrDefaultChildrenOfType<TableNode>();
                            var columnNodes = into.FindChildrenOfType<ColumnNode>();

                            StringBuilder colBuilder = new StringBuilder();

                            foreach (var col in columnNodes)
                            {
                                if (colBuilder.Length > 0)
                                {
                                    colBuilder.Append(", ");
                                }

                                colBuilder.Append(col.ColumnName);
                                callFunction.Arugments.Add(col.ColumnName);
                            }

                            callFunction.FunctionName = table.TableName;
                            callFunction.Type = "_insert_into";

                            strBuilder.Append(string.Format(SIQLCommands.CALL_FUNCTION_PREP, "_insert_into", table.TableName, colBuilder.ToString()));
                        }
                    }
                    break;
                #endregion

                #region [SyntaxNodeType.FuncCall]
                case SyntaxNodeType.FuncCall:
                    {
                        CompileCallFunction(strBuilder, node, parent, intendend);
                    }
                    break;
                #endregion

                #region [SyntaxNodeType.Constant]
                case SyntaxNodeType.Constant:
                    var constNode = (node as ConstantNode);

                    if (constNode.DataType == DataType.Null)
                    {
                        strBuilder.AppendLine("ldnull");
                        var ldNull = parent.CreateNode<LoadConstantCCNode>();
                        ldNull.ConstantValue = null;
                        ldNull.DataType = DataType.Null;
                    }
                    else
                    {
                        var type = GetConstantTypeStr(constNode);
                        string val = node.Token.Content;

                        if (val.EndsWith("i") || val.EndsWith("l") || val.EndsWith("f") || val.EndsWith("d"))
                        {
                            val = val.Substring(0, val.Length - 1);
                        }

                        strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.LOAD_CONST_PREP, type, val));
                        var ldc = parent.CreateNode<LoadConstantCCNode>();
                        ldc.DataType = DataTypeHelper.StrToDataType(type);
                        ldc.ConstantValue = DataTypeHelper.StringValueToObject(val, ldc.DataType);
                    }
                    break;
                #endregion

                #region [SyntaxNodeType.Operator]
                case SyntaxNodeType.Operator:
                    {
                        switch ((node as OperatorNode).OperatorType)
                        {
                            case OperatorType.Add:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_ADD);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.Add;
                                break;
                            case OperatorType.Sub:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_SUB);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.Sub;
                                break;
                            case OperatorType.Mul:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_MUL);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.Mul;
                                break;
                            case OperatorType.Div:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_DIV);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.Div;
                                break;
                            case OperatorType.Equal:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_EQ);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.Equal;
                                break;
                            case OperatorType.Unequal:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_UEQ);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.Unequal;
                                break;
                            case OperatorType.Greater:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_GT);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.Greater;
                                break;
                            case OperatorType.Smaller:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_SM);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.Smaller;
                                break;
                            case OperatorType.GreaterEqual:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_GTEQ);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.GreaterEqual;
                                break;
                            case OperatorType.SmallerEqual:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_SMEQ);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.SmallerEqual;
                                break;
                            case OperatorType.And:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_AND);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.And;
                                break;
                            case OperatorType.Or:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_OR);
                                parent.CreateNode<Runtime.OperatorCCNode>().OpType = OperatorType.Or;
                                break;
                        }
                    }
                    break;
                    #endregion
            }
        }

        private void CompileCallFunction(StringBuilder strBuilder, SyntaxTreeNode node, CommandChainNode parent, int intendend, string functionType = "f")
        {
            string intendendStr = new string('\t', intendend);

            StringBuilder argBuilder = new StringBuilder();
            bool argsFound = false;

            IList<string> argList = new List<string>();

            int i = 0;
            foreach (var args in node.FindChildrenOfType<ArgumentNode>())
            {
                // Parse as expression and push result on the argument stack
                CompileExpression(strBuilder, args, parent, intendend);

                // Crate CChainNode
                parent.CreateNode<LoadArgumentCCNode>().Id = i;
                strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.LOAD_ARGUMENT_PREP, i));

                if (argsFound)
                {
                    argBuilder.Append(", ");
                }

                argBuilder.Append("_dummy_fp" + i.ToString());
                argList.Add("_dummy_fp" + i.ToString());
                argsFound = true;
                i++;
            }

            // Call the function
            strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.CALL_FUNCTION_PREP, functionType, (node as SyntaxTreeNode).Token.Content, argBuilder.ToString()));

            var callFunction = parent.CreateNode<Runtime.CallFunctionCCNode>();
            callFunction.FunctionName = (node as SyntaxTreeNode).Token.Content;
            callFunction.Type = functionType;
            callFunction.Arugments = argList;
        }

        #region [CompileExpression]
        /// <summary>
        /// Compile expressions like ... = 1 - 2 + 3 == 2
        /// </summary>
        /// <param name="strBuilder"></param>
        /// <param name="node"></param>
        /// <param name="intendend"></param>
        private void CompileExpression(StringBuilder strBuilder, SyntaxTreeNode node, CommandChainNode parent, int intendend)
        {
            Dequeue<SyntaxTreeNode> postFixQueue = new Dequeue<SyntaxTreeNode>();

            // Parse the tree to postfix notation and ignore As-Keyowrds
            List<SyntaxTreeNode> tempList = node.FindChildrenBesidesOfType<AsNode>().ToList();

            while (tempList.Count > 0)
            {
                List<SyntaxTreeNode> innerList = new List<SyntaxTreeNode>();

                foreach (SyntaxTreeNode tempNode in tempList.Where(Item => Item.NodeType != SyntaxNodeType.Operator).ToList())
                {
                    postFixQueue.PushFront(tempNode);
                }

                foreach (SyntaxTreeNode tempNode in tempList.Where(Item => Item.NodeType == SyntaxNodeType.Operator).ToList())
                {
                    postFixQueue.PushFront(tempNode);

                    innerList.AddRange(tempNode.Children);
                }

                tempList.Clear();
                tempList = innerList;
            }

            // Compile
            while (postFixQueue.Count > 0)
            {
                var qNode = postFixQueue.PopFirst();

                // Compile tree element
                Compile(strBuilder, qNode, parent, intendend);
            }
        }
        #endregion

        #region [GetConstantTypeStr]
        /// <summary>
        /// Get the type as a string
        /// </summary>
        /// <param name="node">Node instnace</param>
        /// <returns>Type as string</returns>
        public string GetConstantTypeStr(ConstantNode node)
        {
            switch (node.DataType)
            {
                case DataType.None:
                case DataType.Null:
                    break;

                case DataType.Boolean:
                    return "i2";

                case DataType.Int32:
                    return "i4";

                case DataType.Int64:
                    return "i8";

                case DataType.Float32:
                    return "r4";

                case DataType.Float64:
                    return "r8";

                case DataType.Str:
                    return "str";

                default:
                    return "";
            }

            return "";
        }
        #endregion

        private string GetCursorName(int curNo)
        {
            return string.Format("cur{0}", curNo);
        }

        private string GetResultSetName(int setNo)
        {
            return string.Format("res{0}", setNo);
        }

        #endregion
    }
}
