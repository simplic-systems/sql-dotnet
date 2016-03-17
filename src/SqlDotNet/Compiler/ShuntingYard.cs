using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlDotNet.Compiler
{
    internal class ShuntingYard
    {
        #region Private Member
        private IList<SyntaxTreeNode> tokens;
        #endregion

        #region Constructor
        /// <summary>
        /// Create new ShuntingYarg tokenizer
        /// </summary>
        /// <param name="Tokens">List of tokens</param>
        public ShuntingYard(IList<SyntaxTreeNode> Tokens)
        {
            tokens = Tokens;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Execute the algorithm and reorder tokens
        /// </summary>
        /// <returns></returns>
        public IList<SyntaxTreeNode> Execute()
        {
            IList<SyntaxTreeNode> returnValue = new List<SyntaxTreeNode>();
            Stack<SyntaxTreeNode> operatorStack = new Stack<SyntaxTreeNode>();

            /*
             * With help from and thanks to:
             * http://en.wikipedia.org/wiki/Shunting_yard_algorithm
             * http://www.codeproject.com/Tips/351042/Shunting-Yard-algorithm-in-Csharp
             */

            foreach (SyntaxTreeNode token in tokens)
            {
                // Add all tokens here, which should be processed als values.
                // For example: 1 + 1 <-- Const
                //              1 + col1 <-- Column
                //              in (1, 2, 3) <-- ColumnList
                //              max() <-- Function call
                if (token is ConstantNode || token is ColumnNode || token is ReturnValueList || token is CallFunctionNode)
                {
                    returnValue.Add(token);
                }
                else if (TokenIsOperator(token))
                {
                    while (operatorStack.Count > 0)
                    {
                        SyntaxTreeNode currentOpr = operatorStack.Peek();
                        // if ot is operator && o < ot
                        if (TokenIsOperator(currentOpr) && (
                            ((currentOpr as OperatorNode).Association == OperatorAssociation.Left && Precedence((token as OperatorNode), (currentOpr as OperatorNode)) <= 0) ||
                            ((currentOpr as OperatorNode).Association == OperatorAssociation.Right && Precedence((token as OperatorNode), (currentOpr as OperatorNode)) < 0))
                            )
                        {
                            // Add to Output
                            returnValue.Add(operatorStack.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Push the current token (operator) to the operator stack for later usage
                    operatorStack.Push(token);
                }
                else if (token is ParenthesisNode && (token as ParenthesisNode).Type == BracketType.Open)
                {
                    operatorStack.Push(token);
                }
                else if (token is ParenthesisNode && (token as ParenthesisNode).Type == BracketType.Close)
                {
                    bool pe = false;
                    while (operatorStack.Count > 0)
                    {   // opr to out until (
                        SyntaxTreeNode sc = operatorStack.Peek();
                        if (sc is ParenthesisNode && (sc as ParenthesisNode).Type == BracketType.Open)
                        {
                            pe = true;
                            break;
                        }
                        else
                        {
                            returnValue.Add(operatorStack.Pop());
                        }
                    }
                    if (!pe) throw new Exception("No Left (");
                    {
                        operatorStack.Pop();
                    }
                }
            }

            while (operatorStack.Count > 0)
            {
                returnValue.Add(operatorStack.Pop());
            }

            return returnValue;
        }

        /// Check wether current ValueToken is operator
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public static bool TokenIsOperator(SyntaxTreeNode Token)
        {
            return (Token is OperatorNode);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get the precedence difference for two value-token
        /// </summary>
        /// <param name="Operator1"></param>
        /// <param name="Operator2"></param>
        /// <returns>Integer value</returns>
        internal int Precedence(OperatorNode Operator1, OperatorNode Operator2)
        {
            if (Operator1.Precedence > Operator2.Precedence) return 1;
            if (Operator1.Precedence < Operator2.Precedence) return -1;
            return 0;
        }
        #endregion
    }
}
