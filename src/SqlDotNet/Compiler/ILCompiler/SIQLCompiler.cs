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
        public System.IO.Stream Compile(SyntaxTreeNode node)
        {
            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine("// SQL program");
            strBuilder.AppendLine();
            strBuilder.AppendLine("@sqil " + SIQL_VERSION);
            strBuilder.AppendLine();

            // Compile to output
            foreach (var child in node.Children)
            {
                Compile(strBuilder, child, 0);
            }

            // Return code
            return new System.IO.MemoryStream(Encoding.UTF8.GetBytes(strBuilder.ToString()));
        }
        #endregion

        #region Private Member
        private void Compile(StringBuilder strBuilder, SyntaxTreeNode node, int intendend)
        {
            string intendendStr = new string('\t', intendend);

            switch (node.NodeType)
            {
                case SyntaxNodeType.Select:
                    {
                        // Open new cursor
                        cursorCounter++;
                        int cursorNr = cursorCounter;

                        strBuilder.Append(intendendStr + "ocur.");

                        // Cursor type. Currently only none-types and tables are supported
                        bool isNoneCursor = false;
                        string tableName = "";
                        if (node.FindChildrenOfType<FromNode>().Count > 0)
                        {
                            tableName = node.FindChildrenOfType<FromNode>()[0].FindChildrenOfType<TableNode>()[0].TableName;
                            strBuilder.Append("tpl");
                        }
                        else
                        {
                            isNoneCursor = true;
                            strBuilder.Append("none");
                        }

                        // cur0 / curx
                        strBuilder.Append(GetCursorName(cursorNr));

                        // Set cursor source, if type is not none:
                        if (!isNoneCursor)
                        {
                            // cursrc.cur0 B		// Set the cursor source. Syntax cursrc.<name> Source-Name
                            strBuilder.AppendLine(string.Format("cursrc.{0} {1}", GetCursorName(cursorNr), tableName));
                        }

                    }
                    break;

                #region [SyntaxNodeType.FuncCall]
                case SyntaxNodeType.FuncCall:
                    {
                        break;
                        StringBuilder argBuilder = new StringBuilder();
                        argBuilder.Append(intendendStr + "call.f " + (node as SyntaxTreeNode).Token.Content + "(");
                        bool argsFound = false;

                        int i = 0;
                        /*foreach (var args in node.FindChildrenOfType<ArgumentNode>())
                        {
                            // Parse as expression and push result on the argument stack
                            CompileExpression(strBuilder, args, intendend);
                            strBuilder.AppendLine((new string('\t', intendend)) + "ldarg." + i.ToString());

                            if (argsFound)
                            {
                                argBuilder.Append(", ");
                            }

                            argBuilder.Append("_dummy_fp" + i.ToString());

                            argsFound = true;
                            i++;
                        }*/

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

                        strBuilder.AppendLine(intendendStr + "ldc." + type + " " + val);
                    }
                    break;
                #endregion

                #region [SyntaxNodeType.Operator]
                case SyntaxNodeType.Operator:
                    {
                        switch ((node as OperatorNode).OperatorType)
                        {
                            case OperatorType.Add:
                                strBuilder.AppendLine(intendendStr + "add");
                                break;
                            case OperatorType.Sub:
                                strBuilder.AppendLine(intendendStr + "sub");
                                break;
                            case OperatorType.Mul:
                                strBuilder.AppendLine(intendendStr + "mul");
                                break;
                            case OperatorType.Div:
                                strBuilder.AppendLine(intendendStr + "div");
                                break;
                            case OperatorType.Equal:
                                strBuilder.AppendLine(intendendStr + "eq");
                                break;
                            case OperatorType.Unequal:
                                strBuilder.AppendLine(intendendStr + "ueq");
                                break;
                            case OperatorType.Greater:
                                strBuilder.AppendLine(intendendStr + "gt");
                                break;
                            case OperatorType.Smaller:
                                strBuilder.AppendLine(intendendStr + "sm");
                                break;
                            case OperatorType.GreaterEqual:
                                strBuilder.AppendLine(intendendStr + "gteq");
                                break;
                            case OperatorType.SmallerEqual:
                                strBuilder.AppendLine(intendendStr + "smeq");
                                break;
                            case OperatorType.And:
                                strBuilder.AppendLine(intendendStr + "and");
                                break;
                            case OperatorType.Or:
                                strBuilder.AppendLine(intendendStr + "or");
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

            // Parse the tree to postfix notation
            List<SyntaxTreeNode> tempList = node.Children.ToList();

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

        #endregion
    }
}
