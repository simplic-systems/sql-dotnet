using Simplic.Collections.Generic;
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
        #endregion

        #region Constructor
        /// <summary>
        /// Create compiler
        /// </summary>
        /// <param name="errorListener">Error listener instance</param>
        public SIQLCompiler(IErrorListener errorListener)
        {
            this.errorListener = errorListener;
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
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine("// SQL program");
            strBuilder.AppendLine();
            strBuilder.AppendLine(string.Format("{0} {1}", SIQLCommands.SIQL_TAG, SIQL_VERSION));
            strBuilder.AppendLine();

            // Compile to output
            foreach (var child in node.Children)
            {
                Compile(strBuilder, child, 0);
            }

            // Return code
            result.ILCode = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(strBuilder.ToString()));
            result.EntryPoint = node as EntryPointNode;
            return result;
        }
        #endregion

        #region Private Member
        private void Compile(StringBuilder strBuilder, SyntaxTreeNode node, int intendend)
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
                            // Open new cursor
                            cursorCounter++;
                            int cursorNr = cursorCounter;
                            cursorName = GetCursorName(cursorNr);
                            strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.CURSOR_OPEN_PREP, "tbl", cursorName));

                            // Output definition
                            strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.CURSOR_OUTPUT_DEFINITION_PREP, cursorName, ""));

                            // Cursor source
                            string tableName = table.TableName;
                            if (!string.IsNullOrWhiteSpace(table.Owner))
                            {
                                tableName = table.Owner + "." + table.TableName;
                            }

                            strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.CURSOR_SOURCE_PREP, cursorName, tableName));

                            // Append filter
                            var whereNode = node.FindFirstOrDefaultChildrenOfType<WhereNode>();

                            if (whereNode != null)
                            {
                                strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.CURSOR_FILTER_PREP, cursorName));
                                strBuilder.AppendLine(intendendStr + "{");

                                CompileExpression(strBuilder, node.FindFirstOrDefaultChildrenOfType<WhereNode>(), intendend + 1);

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
                        foreach (var _node in lst.Children)
                        {
                            var alias = _node.FindFirstOrDefaultChildrenOfType<AsNode>();

                            if (resultSetDefinition.Length > 0)
                            {
                                resultSetDefinition.Append(", ");
                            }

                            if (alias == null || string.IsNullOrWhiteSpace(alias.Alias))
                            {
                                resultSetDefinition.Append(string.Format("__col{0}", unnamedColumns));
                                unnamedColumns++;
                            }
                            else
                            {
                                resultSetDefinition.Append(alias.Alias);
                            }
                        }

                        strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.RESULTSET_OPEN_PREP, resultSetName, "(" + resultSetDefinition.ToString() + ")"));

                        strBuilder.AppendLine(intendendStr + string.Format(SIQLCommands.RESULTSET_FILL_PREP, resultSetName, string.Format("({0})", (cursorName ?? ""))));
                        strBuilder.AppendLine(intendendStr + "{");

                        unnamedColumns = 0;
                        if (lst != null)
                        {
                            strBuilder.AppendLine("\t" + intendendStr + SIQLCommands.RESULTSET_CREATE_ROW);

                            foreach (var _node in lst.Children)
                            {
                                // Some dummy node, only needed because CompileExpression iterates over children
                                // and needs a kind of "host"
                                var __d__ = new DummyNode(_node);

                                CompileExpression(strBuilder, __d__, intendend + 1);

                                var alias = _node.FindFirstOrDefaultChildrenOfType<AsNode>();
                                string aliasStr = "";
                                if (alias != null && !string.IsNullOrWhiteSpace(alias.Alias))
                                {
                                    aliasStr = alias.Alias;
                                }
                                else
                                {
                                    aliasStr = string.Format("__col{0}", unnamedColumns);
                                    unnamedColumns++;
                                }

                                strBuilder.AppendLine("\t" + intendendStr + string.Format(SIQLCommands.RESULTSET_POP_TO_NEXT_COLUMN_REP, aliasStr));
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
                    }
                    break;
                #endregion   

                #region [SyntaxNodeType.FuncCall]
                case SyntaxNodeType.FuncCall:
                    {
                        StringBuilder argBuilder = new StringBuilder();
                        argBuilder.Append(intendendStr + string.Format(SIQLCommands.CALL_FUNCTION_PREP, "f", (node as SyntaxTreeNode).Token.Content) + "(");
                        bool argsFound = false;

                        int i = 0;
                        foreach (var args in node.FindChildrenOfType<ArgumentNode>())
                        {
                            // Parse as expression and push result on the argument stack
                            CompileExpression(strBuilder, args, intendend);
                            strBuilder.AppendLine((new string('\t', intendend)) + string.Format(SIQLCommands.LOAD_ARGUMENT_PREP, i));

                            if (argsFound)
                            {
                                argBuilder.Append(", ");
                            }

                            argBuilder.Append("_dummy_fp" + i.ToString());

                            argsFound = true;
                            i++;
                        }

                        argBuilder.Append(")");

                        // Call the function
                        strBuilder.AppendLine(argBuilder.ToString());
                    }
                    break;
                #endregion

                #region [SyntaxNodeType.Constant]
                case SyntaxNodeType.Constant:
                    var constNode = (node as ConstantNode);

                    if (constNode.DataType == DataType.Null)
                    {
                        strBuilder.AppendLine("ldnull");
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
                                break;
                            case OperatorType.Sub:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_SUB);
                                break;
                            case OperatorType.Mul:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_MUL);
                                break;
                            case OperatorType.Div:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_DIV);
                                break;
                            case OperatorType.Equal:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_EQ);
                                break;
                            case OperatorType.Unequal:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_UEQ);
                                break;
                            case OperatorType.Greater:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_GT);
                                break;
                            case OperatorType.Smaller:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_SM);
                                break;
                            case OperatorType.GreaterEqual:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_GTEQ);
                                break;
                            case OperatorType.SmallerEqual:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_SMEQ);
                                break;
                            case OperatorType.And:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_AND);
                                break;
                            case OperatorType.Or:
                                strBuilder.AppendLine(intendendStr + SIQLCommands.OP_OR);
                                break;
                        }
                    }
                    break;
                    #endregion
            }
        }

        #region [CompileExpression]
        /// <summary>
        /// Compile expressions like ... = 1 - 2 + 3 == 2
        /// </summary>
        /// <param name="strBuilder"></param>
        /// <param name="node"></param>
        /// <param name="intendend"></param>
        private void CompileExpression(StringBuilder strBuilder, SyntaxTreeNode node, int intendend)
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
                Compile(strBuilder, qNode, intendend);
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
